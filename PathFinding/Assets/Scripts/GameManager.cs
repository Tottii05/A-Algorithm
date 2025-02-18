using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int Size;
    public BoxCollider2D Panel;
    public GameObject token;
    private Node[,] NodeMatrix;
    [SerializeField] private int startPosx, startPosy;
    [SerializeField] private int endPosx, endPosy;

    void Awake()
    {
        Instance = this;
        Calculs.CalculateDistances(Panel, Size);
    }
    private void Start()
    {
        startPosx = Random.Range(0, Size);
        startPosy = Random.Range(0, Size);
        do
        {
            endPosx = Random.Range(0, Size);
            endPosy = Random.Range(0, Size);
        } while (endPosx == startPosx || endPosy == startPosy);
        NodeMatrix = new Node[Size, Size];
        CreateNodes();
        AStarAlgorithm();
    }
    public void CreateNodes()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                NodeMatrix[i, j] = new Node(i, j, Calculs.CalculatePoint(i, j));
                NodeMatrix[i, j].Heuristic = Calculs.CalculateHeuristic(NodeMatrix[i, j], endPosx, endPosy);
            }
        }
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                SetWays(NodeMatrix[i, j], i, j);
            }
        }
        DebugMatrix();
    }
    public void DebugMatrix()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                GameObject instance = Instantiate(token, NodeMatrix[i, j].RealPosition, Quaternion.identity);
                if (i == startPosx && j == startPosy)
                {
                    instance.GetComponent<SpriteRenderer>().color = Color.green;
                }
                if (i == endPosx && j == endPosy)
                {
                    instance.GetComponent<SpriteRenderer>().color = Color.blue;
                }
                instance.GetComponentsInChildren<TextMeshProUGUI>()[0].text = NodeMatrix[i, j].Heuristic.ToString();
            }
        }
    }
    public void SetWays(Node node, int x, int y)
    {
        node.WayList = new List<Way>();
        if (x > 0)
        {
            node.WayList.Add(new Way(NodeMatrix[x - 1, y], Calculs.LinearDistance));
            if (y > 0)
            {
                node.WayList.Add(new Way(NodeMatrix[x - 1, y - 1], Calculs.DiagonalDistance));
            }
        }
        if (x < Size - 1)
        {
            node.WayList.Add(new Way(NodeMatrix[x + 1, y], Calculs.LinearDistance));
            if (y > 0)
            {
                node.WayList.Add(new Way(NodeMatrix[x + 1, y - 1], Calculs.DiagonalDistance));
            }
        }
        if (y > 0)
        {
            node.WayList.Add(new Way(NodeMatrix[x, y - 1], Calculs.LinearDistance));
        }
        if (y < Size - 1)
        {
            node.WayList.Add(new Way(NodeMatrix[x, y + 1], Calculs.LinearDistance));
            if (x > 0)
            {
                node.WayList.Add(new Way(NodeMatrix[x - 1, y + 1], Calculs.DiagonalDistance));
            }
            if (x < Size - 1)
            {
                node.WayList.Add(new Way(NodeMatrix[x + 1, y + 1], Calculs.DiagonalDistance));
            }
        }
    }
    public void AStarAlgorithm()
    {
        Node CurrentNode = NodeMatrix[startPosx, startPosy];
        Node endNode = NodeMatrix[endPosx, endPosy];
        Dictionary<Node, float> OpenList = new Dictionary<Node, float>() { { CurrentNode, 0 } };
        List<Node> ClosedList = new List<Node>();
        float aCUMulatedCost = 0;

        while (CurrentNode != endNode)
        {
            aCUMulatedCost = OpenList[CurrentNode];
            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);
            foreach (var way in CurrentNode.WayList)
            {
                if (!ClosedList.Contains(way.NodeDestiny))
                {
                    bool visited = false;
                    foreach (var node in OpenList)
                    {
                        if (node.Key == way.NodeDestiny)
                        {
                            visited = true;
                        }
                    }
                    if (!visited)
                    {
                        way.NodeDestiny.NodeParent = CurrentNode;
                        OpenList.Add(way.NodeDestiny, aCUMulatedCost + way.Cost);
                    }
                }
            }
            CurrentNode = OpenList.OrderBy(x => x.Value + x.Key.Heuristic).First().Key;
        }
        StartCoroutine(PaintNodes(ClosedList, NodeMatrix[startPosx, startPosy]));
    }

    public IEnumerator PaintNodes(List<Node> list, Node Start)
    {
        foreach (var node in list)
        {
            if (node != Start)
            {
                GameObject instance = Instantiate(token, node.RealPosition, Quaternion.identity);
                instance.GetComponent<SpriteRenderer>().color = Color.magenta;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
