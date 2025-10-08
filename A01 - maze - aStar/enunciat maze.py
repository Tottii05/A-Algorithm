import pyamaze as maze
import time
from queue import PriorityQueue

ROWS = 50
COLS = 50

def distance(cell1, cell2):
    return abs(cell1[0] - cell2[0]) + abs(cell1[1] - cell2[1])

def aStar(m):
    start = (m.rows, m.cols)
    objetivo = (1, 1)
    nodosAExplorar = PriorityQueue()
    nodosAExplorar.put((0, start))
    celdas = {cell: float('inf') for cell in m.grid}
    celdas[start] = 0
    camino = {}

    while not nodosAExplorar.empty():
        celdaActual = nodosAExplorar.get()[1]
        if celdaActual == objetivo:
            caminoFinal = []
            while celdaActual in camino:
                caminoFinal.append(celdaActual)
                celdaActual = camino[celdaActual]
            caminoFinal.append(start)
            return caminoFinal[::-1]
        for puntoCardinal in 'ESNW':
            if m.maze_map[celdaActual][puntoCardinal] == True:
                if puntoCardinal == 'E':
                    vecino = (celdaActual[0], celdaActual[1] + 1)
                if puntoCardinal == 'W':
                    vecino = (celdaActual[0], celdaActual[1] - 1)
                if puntoCardinal == 'N':
                    vecino = (celdaActual[0] - 1, celdaActual[1])
                if puntoCardinal == 'S':
                    vecino = (celdaActual[0] + 1, celdaActual[1])
                print(f"Desde {celdaActual} hacia {puntoCardinal} -> vecino: {vecino}")
                mejorPuntuacionActual = celdas[celdaActual] 
                if mejorPuntuacionActual < celdas[vecino]:
                    celdas[vecino] = mejorPuntuacionActual
                    caminoPrioritario = mejorPuntuacionActual + distance(vecino, objetivo)
                    nodosAExplorar.put((caminoPrioritario, vecino))
                    camino[vecino] = celdaActual
    return []


m=maze.maze(ROWS,COLS)
m.CreateMaze()
pre_Astar = time.time()
path = aStar(m)
print(path)
post_Astar = time.time()
print(post_Astar - pre_Astar)
a=maze.agent(m,footprints=True, shape='arrow', color='green')
m.tracePath({a:path},delay=300)
m.run()