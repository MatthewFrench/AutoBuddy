﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Color = System.Drawing.Color;

namespace AutoBuddy.Utilities.Pathfinder
{
    internal class NavGraph
    {
        public Node[] Nodes;
        public readonly ColorBGRA NodeColor;
        public readonly Color LineColor;
        private readonly string navFile;
        private NavGraphTest test;

        public NavGraph(string dir)
        {
            navFile = Path.Combine(dir, "NavGraph" + Game.MapId + ".txt");
            NodeColor = new ColorBGRA(50, 200, 0, 255);
            LineColor = Color.Gold;
            Load();
            Chat.OnInput += Chat_OnInput;
        }

        void Chat_OnInput(ChatInputEventArgs args)
        {
            if (test != null || !args.Input.Equals("/navgraph"))
            {
                return;
            }
            args.Process = false;
            test = new NavGraphTest(this);
        
        }

        public void Save()
        {
            using (var f = new FileStream(navFile, FileMode.Create, FileAccess.Write))
            {
                f.Write(BitConverter.GetBytes(Nodes.Length), 0, 4);
                foreach (var node in Nodes)
                {
                    node.Serialize(f);
                }
            }
        }



        private void load(string file)
        {
            var buffer = new byte[4];
            using (var f = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                f.Read(buffer, 0, 4);
                Nodes=new Node[BitConverter.ToInt32(buffer, 0)];
                for (var i = 0; i < Nodes.Length; i++)
                {
                    Nodes[i]=new Node(f, buffer, this);
                }
            }
        }

        private void load()
        {
            var buffer = new byte[4];
            using (var f = new MemoryStream(BrutalExtensions.GetResourceForGame()))
            {
                f.Read(buffer, 0, 4);
                Nodes = new Node[BitConverter.ToInt32(buffer, 0)];
                for (var i = 0; i < Nodes.Length; i++)
                {
                    Nodes[i] = new Node(f, buffer, this);
                }
            }
        }

        public void Load()
        {
            if (!File.Exists(navFile))
                load();
            else
                load(navFile);
        }

        public void AddNode(Vector3 pos)
        {
            var tmp = new Node[Nodes.Length + 1];
            Nodes.CopyTo(tmp, 0);
            Nodes = tmp;
            Nodes[Nodes.Length - 1] = new Node(new int[0], pos, this);

        }
        public void RemoveNode(int node)
        {
            while (Nodes[node].Neighbors.Length > 0)
            {
                RemoveLink(node, Nodes[node].Neighbors[0]);
            }
            var tmp = new Node[Nodes.Length - 1];
            Array.Copy(Nodes, 0, tmp, 0, node);
            Array.Copy(Nodes, node + 1, tmp, node, Nodes.Length - node - 1);
            Nodes = tmp;
            for (var i = 0; i < Nodes.Length; i++)
            {
                for (var j = 0; j < Nodes[i].Neighbors.Length; j++)
                {
                    if (Nodes[i].Neighbors[j] > node)
                        Nodes[i].Neighbors[j]--;
                }
            }
        }

        public int FindClosestNode(Vector3 pos)
        {
            var minDist = float.MaxValue;
            var node = -1;
            for (var i = 0; i < Nodes.Length; i++)
            {
                if (!(pos.Distance(Nodes[i].position) < minDist)) continue;
                minDist = pos.Distance(Nodes[i].position);
                node = i;
            }
            return node;
        }
        public int FindClosestNode(Vector3 pos, Vector3 end)
        {
            var minDist = float.MaxValue;
            var node = -1;
            for (var i = 0; i < Nodes.Length; i++)
            {
                if (!(pos.Distance(Nodes[i].position) < minDist) || end.Distance(Nodes[i].position) > end.Distance(ObjectManager.Player)) continue;
                minDist = pos.Distance(Nodes[i].position);
                node = i;
            }
            return node;
        }

        public int FindClosestNode(Vector3 pos, int except)
        {
            var minDist = float.MaxValue;
            var node = -1;
            for (var i = 0; i < Nodes.Length; i++)
            {
                if (except == i || !(pos.Distance(Nodes[i].position) < minDist || !Nodes[i].passable)) continue;
                minDist = pos.Distance(Nodes[i].position);
                node = i;
            }
            return node;
        }

        public int FindClosestNode(int nodeId)
        {

            var minDist = float.MaxValue;
            var node = 0;
            for (var i = 0; i < Nodes.Length; i++)
            {
                if (nodeId == i || Nodes[nodeId].position.Distance(Nodes[i].position) > minDist||!Nodes[i].passable) continue;
                minDist = Nodes[nodeId].position.Distance(Nodes[i].position);
                node = i;
            }
            return node;
        }

        public void AddLink(int node1, int node2)
        {
            Nodes[node1].AddNeighbor(node2);
            Nodes[node2].AddNeighbor(node1);
        }
        public void RemoveLink(int node1, int node2)
        {
            Nodes[node1].RemoveNeighbor(node2);
            Nodes[node2].RemoveNeighbor(node1);
        }
        public bool LinkExists(int node1, int node2)
        {
            return Nodes[node1].Neighbors.Contains(node2);
        }

        public void Draw()
        {
            for (var i = 0; i < Nodes.Length; i++)
            {
                Nodes[i].DrawPositions();
                //if(Nodes[i].position.IsOnScreen())
                   // Drawing.DrawText(Nodes[i].position.WorldToScreen(), Color.Gold, "      " + i + "    " + Nodes[i].Neighbors.Length, 10);
            }
            for (var i = 0; i < Nodes.Length; i++)
            {
                Nodes[i].DrawLinks();
            }

        }

        public List<Vector3> FindPath(Vector3 start, Vector3 end)
        {
            var p = FindPath(FindClosestNode(start), FindClosestNode(end));
            var ret = new List<Vector3> {end};
            while (p!=null)
            {
                ret.Add(Nodes[p.node].position);
                p = p.Parent;
            }
            ret.Reverse();
            return ret;
        }
        public List<Vector3> FindPath2(Vector3 start, Vector3 end)
        {
            var p = FindPath(FindClosestNode(start, end), FindClosestNode(end));
            var ret = new List<Vector3> { end };
            while (p != null)
            {
                ret.Add(Nodes[p.node].position);
                p = p.Parent;
            }
            ret.Reverse();
            return ret;
        }
        public List<Vector3> FindPathRandom(Vector3 start, Vector3 end)
        {
            var p = FindPath(FindClosestNode(start, end), FindClosestNode(end));
            var ret = new List<Vector3> { end };
            while (p != null)
            {
                ret.Add(Nodes[p.node].position.Randomized(-15f, 15f));
                p = p.Parent;
            }
            ret.Reverse();
            return ret;
        }

        private PathNode FindPath(int startNode, int endNode)
        {
            if (startNode == -1 || endNode == -1) return null;
            var open=new List<PathNode>();
            var closed = new List<PathNode>();
            open.Add(new PathNode(0, Nodes[startNode].GetDistance(endNode), startNode, null));


            while (open.Count > 0)
            {
                var q = open.OrderBy(n => n.fCost).First();
                open.Remove(q);
                foreach (var neighbor in Nodes[q.node].Neighbors)
                {
                    if (!Nodes[neighbor].passable)
                        continue;
                    var s=new PathNode(q.gCost+
                        Distance(q, neighbor) * (Nodes[neighbor].position.GetNearestTurret().Distance(Nodes[neighbor].position)<900?20:1)
                        
                        
                        ,Distance(endNode, neighbor), neighbor, q );
                    if (neighbor == endNode)
                    {
                        return s;
                    }
                    var sameNodeOpen = open.FirstOrDefault(el => el.node == neighbor);
                    if (sameNodeOpen != null)
                    {
                        if(sameNodeOpen.fCost<s.fCost) continue;
                    }
                    var sameNodeClosed = closed.FirstOrDefault(el => el.node == neighbor);
                    if (sameNodeClosed != null)
                    {
                        if(sameNodeClosed.fCost<s.fCost) continue;
                    }
                    open.Add(s);
                }
                closed.Add(q);
            }
            return null;

        }


        private float Distance(PathNode p1, int p2)
        {
            return Nodes[p1.node].position.Distance(Nodes[p2].position);
        }

        private float Distance(int p1, int p2)
        {
            return Nodes[p1].position.Distance(Nodes[p2].position);
        }

    }
}
