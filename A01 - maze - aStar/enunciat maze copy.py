import pyamaze as maze
import time
from queue import PriorityQueue

ROWS = 20
COLS = 20

def distance(cell1, cell2):
    return abs(cell1[0] - cell2[0]) + abs(cell1[1] - cell2[1])

def aStar(m):
    inicio = (m.rows, m.cols)
    final = (1,1)
    caminosAExplorar = PriorityQueue()
    caminosAExplorar.put((0, inicio))
    celdas = {cell: float('inf') for cell in m.grid}
    celdas[inicio] = 0
    camino = {}

    while not caminosAExplorar.empty():
        celdaActual = caminosAExplorar.get()[1]
        if celdaActual == final:
            caminoFinal = []
            while celdaActual in camino:
                caminoFinal.append(celdaActual)
                celdaActual = camino[celdaActual]
            caminoFinal.append(inicio)
            return caminoFinal[::-1]
        for puntoCardinal in 'EWNS':
            if m.maze_map[celdaActual][puntoCardinal] == True:
                if puntoCardinal == 'E':
                    vecino = (celdaActual[0], celdaActual[1] + 1)
                if puntoCardinal == 'W':
                    vecino = (celdaActual[0], celdaActual[1] - 1)
                if puntoCardinal == 'N':
                    vecino = (celdaActual[0] - 1, celdaActual[1])
                if puntoCardinal == 'S':
                    vecino = (celdaActual[0] + 1, celdaActual[1])
                
                if vecino in m.grid:
                    mejorCamino = celdas[celdaActual] + 1
                    if mejorCamino < celdas[vecino]:
                        celdas[vecino] = mejorCamino
                        preferencia = mejorCamino + distance(vecino, final)
                        caminosAExplorar.put((preferencia, vecino))
                        camino[vecino] = celdaActual

    return []
        
m=maze.maze(ROWS,COLS)
m.CreateMaze()
pre_Astar = time.time()
path = aStar(m)
post_Astar = time.time()
print(post_Astar - pre_Astar)
a=maze.agent(m,footprints=True, shape='arrow', color='green')
m.tracePath({a:path},delay=4)
m.run()