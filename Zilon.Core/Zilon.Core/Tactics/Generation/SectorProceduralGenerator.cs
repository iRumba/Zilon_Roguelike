﻿using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Generation
{
    public class SectorProceduralGenerator
    {
        private const int ROOM_COUNT = 10;
        private const int ROOM_CELL_SIZE = 10;

        private readonly ISectorGeneratorRandomSource _randomSource;

        public SectorProceduralGenerator(ISectorGeneratorRandomSource randomSource)
        {
            _randomSource = randomSource;
        }

        public void Generate(ISector sector, IMap map, IActor playerActor)
        {
            // Генерируем комнаты в сетке
            var roomGridSize = (int)Math.Ceiling(Math.Log(ROOM_COUNT, 2)) + 1;
            var roomGrid = new Room[roomGridSize, roomGridSize];
            var rooms = new List<Room>();

            for (var i = 0; i < ROOM_COUNT; i++)
            {
                var attemptCounter = 3;
                while (true)
                {
                    var rolledPosition = _randomSource.RollRoomPosition(roomGridSize);

                    var currentRoom = roomGrid[rolledPosition.X, rolledPosition.Y];
                    if (currentRoom == null)
                    {
                        var room = new Room
                        {
                            PositionX = rolledPosition.X,
                            PositionY = rolledPosition.Y
                        };

                        var rolledSize = _randomSource.RollRoomSize(ROOM_CELL_SIZE);

                        room.Width = rolledSize.Width;
                        room.Height = rolledSize.Height;

                        rooms.Add(room);
                        break;
                    }

                    attemptCounter--;
                    if (attemptCounter <= 0)
                    {
                        throw new Exception("Не удалось выбрать ячейку для комнаты.");
                    }
                }
            }

            // Создаём сетку комнат
            foreach (var room in rooms)
            {
                for (var x = 0; x < room.Width; x++)
                {
                    for (var y = 0; y < room.Height; y++)
                    {
                        var nodeX = x + room.PositionX * ROOM_CELL_SIZE;
                        var nodeY = y + room.PositionY * ROOM_CELL_SIZE;
                        var node = new HexNode(nodeX, nodeY);
                        room.Nodes.Add(node);
                        map.Nodes.Add(node);

                        var neighbors = HexNodeHelper.GetNeighbors(node, room.Nodes);

                        foreach (var neighbor in neighbors)
                        {
                            var currentEdge = GetExistsEdge(map, node, neighbor);

                            if (currentEdge != null)
                            {
                                continue;
                            }

                            AddEdgeToMap(map, node, neighbor);
                        }
                    }
                }
            }

            // Соединяем комнаты
            foreach(var room in rooms)
            {
                // для каждой комнаты выбираем произвольную другую комнату
                // и проводим к ней коридор

                var selectedRooms = _randomSource.RollConnectedRooms(room, rooms);
                if (selectedRooms == null)
                {
                    //Значит текущая комната тупиковая
                    continue;
                }

                foreach (var selectedRoom in selectedRooms)
                {
                    var startNode = room.Nodes.First();
                    var finishNode = selectedRoom.Nodes.First();

                    //Строим коридор
                    var currentX = startNode.OffsetX;
                    var currentY = startNode.OffsetY;
                    while (true)
                    {
                        if (currentX >= finishNode.OffsetX)
                        {
                            currentX--;
                        }
                        else
                        {
                            currentX++;
                        }

                        if (currentY >= finishNode.OffsetY)
                        {
                            currentY--;
                        }
                        else
                        {
                            currentY++;
                        }

                        var currentNode = map.Nodes.OfType<HexNode>()
                            .SingleOrDefault(x => x.OffsetX == currentX && x.OffsetY == currentY);

                        if (currentNode == null)
                        {
                            currentNode = new HexNode(currentX, currentY);
                            map.Nodes.Add(currentNode);
                        }

                        var currentEdge = (from edge in map.Edges
                                           where edge.Nodes.Contains(startNode)
                                           where edge.Nodes.Contains(currentNode)
                                           select edge).SingleOrDefault();

                        if (currentEdge == null)
                        {
                            currentEdge = new Edge(startNode, currentNode);
                            map.Edges.Add(currentEdge);
                        }

                        startNode = currentNode;
                    }
                }
            }
        }

        /// <summary>
        /// Создаёт на карте ребро, соединяющее два узла этой карты.
        /// </summary>
        /// <param name="targetMap"> Целевая карта, для которой нужно создать ребро. </param>
        /// <param name="node"> Исходное ребро карты. </param>
        /// <param name="neighbor"> Соседнее ребро карты, с которым будет соединено исходное. </param>
        private static void AddEdgeToMap(IMap targetMap, HexNode node, HexNode neighbor)
        {
            var edge = new Edge(node, neighbor);
            targetMap.Edges.Add(edge);
        }

        /// <summary>
        /// Возвращает ребро, соединяющее указанные узлы.
        /// </summary>
        /// <param name="map"> Карта, в которой проверяются ребра. </param>
        /// <param name="node"> Искомый узел. </param>
        /// <param name="neighbor"> Узел, с которым соединён искомый. </param>
        /// <returns> Ребро или null, если такого ребра нет на карте. </returns>
        private static Edge GetExistsEdge(IMap map, HexNode node, HexNode neighbor)
        {
            return (Edge)(from edge in map.Edges
                          where edge.Nodes.Contains(node)
                          where edge.Nodes.Contains(neighbor)
                          select edge).SingleOrDefault();
        }
    }
}