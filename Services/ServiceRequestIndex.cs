using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesMVC.Models;

namespace MunicipalServicesMVC.Services
{
    /// <summary>
    /// Helper/worker class that exposes indexes over service requests (Issues).
    /// Builds:
    /// - a Binary Search Tree keyed by Issue.Id
    /// - a priority heap keyed by Issue.Priority
    /// - a simple graph of locations with a BFS-based inspection route (spanning tree / MST)
    /// </summary>
    public sealed class ServiceRequestIndex
    {
        private readonly List<Issue> _issues;

        /// <summary>
        /// Read-only view of all issues used to build indexes.
        /// </summary>
        public IReadOnlyList<Issue> Issues => _issues;

        // =========================
        // Binary Search Tree by Id
        // =========================

        private sealed class BstNode
        {
            public Issue Value { get; }
            public BstNode? Left { get; set; }
            public BstNode? Right { get; set; }

            public BstNode(Issue value)
            {
                Value = value;
            }
        }

        private BstNode? _root;

        // ==========
        // Heap (by priority: 1 = highest)
        // ==========

        private readonly PriorityQueue<Issue, int> _priorityHeap;

        // ==========
        // Graph of locations (adjacency list)
        // ==========

        private readonly Dictionary<string, HashSet<string>> _locationGraph;

        /// <summary>
        /// Creates a new index over the given issues,
        /// building the BST, heap, and location graph.
        /// </summary>
        public ServiceRequestIndex(IEnumerable<Issue> issues)
        {
            _issues = new List<Issue>(issues ?? Enumerable.Empty<Issue>());
            _priorityHeap = new PriorityQueue<Issue, int>();
            _locationGraph = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var issue in _issues)
            {
                InsertIntoTree(issue);

                // In the heap, smaller priority value = more urgent (1 = highest).
                _priorityHeap.Enqueue(issue, issue.Priority);
            }

            BuildLocationGraph();
        }

        private void InsertIntoTree(Issue issue)
        {
            if (_root == null)
            {
                _root = new BstNode(issue);
                return;
            }

            var current = _root;

            while (true)
            {
                int cmp = string.CompareOrdinal(issue.Id, current.Value.Id);

                // Insert to the left when Id is "smaller" or equal
                if (cmp <= 0)
                {
                    if (current.Left == null)
                    {
                        current.Left = new BstNode(issue);
                        return;
                    }

                    current = current.Left;
                }
                else
                {
                    if (current.Right == null)
                    {
                        current.Right = new BstNode(issue);
                        return;
                    }

                    current = current.Right;
                }
            }
        }

        // =========================
        // Location graph + route
        // =========================

        /// <summary>
        /// Normalises location names for use as graph nodes.
        /// </summary>
        private static string NormalizeLocation(string? location)
        {
            return (location ?? string.Empty).Trim();
        }

        /// <summary>
        /// Builds a simple undirected graph of locations.
        /// Each distinct location becomes a node.
        /// Edges connect consecutive locations in the list, forming
        /// a simple route that can be traversed with BFS.
        /// </summary>
        private void BuildLocationGraph()
        {
            var distinctLocations = _issues
                .Select(i => NormalizeLocation(i.Location))
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Ensure all nodes exist
            foreach (var loc in distinctLocations)
            {
                if (!_locationGraph.ContainsKey(loc))
                {
                    _locationGraph[loc] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }
            }

            // Connect consecutive locations: L0-L1-L2-... (a simple path)
            for (int i = 1; i < distinctLocations.Count; i++)
            {
                var a = distinctLocations[i - 1];
                var b = distinctLocations[i];

                _locationGraph[a].Add(b);
                _locationGraph[b].Add(a);
            }
        }

        /// <summary>
        /// Returns a snapshot of the location graph (adjacency list).
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> GetLocationGraphSnapshot()
        {
            return _locationGraph.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyCollection<string>)kvp.Value.ToList(),
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Uses a BFS traversal over the location graph to build
        /// an inspection route as a spanning tree.
        /// Because all edges are treated as equal cost, this is
        /// also a minimum spanning tree over the graph.
        /// Each tuple (From, To) represents one step in the route.
        /// </summary>
        public IEnumerable<(string From, string To)> GetInspectionRoute()
        {
            if (_locationGraph.Count == 0)
                yield break;

            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<string>();

            // Start from the first location in the graph
            var start = _locationGraph.Keys.First();
            visited.Add(start);
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (!_locationGraph.TryGetValue(current, out var neighbours))
                    continue;

                foreach (var neighbour in neighbours)
                {
                    if (visited.Contains(neighbour))
                        continue;

                    visited.Add(neighbour);
                    queue.Enqueue(neighbour);

                    // This edge is part of the spanning tree / route
                    yield return (current, neighbour);
                }
            }
        }

        // =========================
        // BST: ordered listing
        // =========================

        /// <summary>
        /// Returns all issues ordered by Id using an in-order traversal
        /// of the binary search tree.
        /// </summary>
        public IEnumerable<Issue> GetIssuesOrderedById()
        {
            return TraverseInOrder(_root);

            static IEnumerable<Issue> TraverseInOrder(BstNode? node)
            {
                if (node == null)
                    yield break;

                foreach (var left in TraverseInOrder(node.Left))
                    yield return left;

                yield return node.Value;

                foreach (var right in TraverseInOrder(node.Right))
                    yield return right;
            }
        }

        // =========================
        // Heap: top priority issues
        // =========================

        /// <summary>
        /// Returns the top N highest-priority issues using the heap.
        /// 1 = highest priority, 5 = lowest.
        /// </summary>
        public IEnumerable<Issue> GetTopPriorityIssues(int count)
        {
            if (count <= 0 || _issues.Count == 0)
                yield break;

            // Copy the heap so we don't destroy the original when dequeuing.
            var copy = new PriorityQueue<Issue, int>(
                _priorityHeap.UnorderedItems.Select(item => (item.Element, item.Priority)));

            var taken = 0;
            while (copy.Count > 0 && taken < count)
            {
                yield return copy.Dequeue();
                taken++;
            }
        }

        // =========================
        // Search by reference Id
        // =========================

        /// <summary>
        /// Finds a single issue by its reference ID.
        /// Supports both the full internal Id (GUID)
        /// and the short 8-character reference shown to the user.
        /// </summary>
        public Issue? FindById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var trimmed = id.Trim();
            var trimmedUpper = trimmed.ToUpperInvariant();

            return _issues.FirstOrDefault(i =>
            {
                // 1) full internal Id match
                if (string.Equals(i.Id, trimmed, StringComparison.OrdinalIgnoreCase))
                    return true;

                // 2) short public code match (first 8 chars)
                if (!string.IsNullOrEmpty(i.Id) && i.Id.Length >= 8)
                {
                    var shortCode = i.Id.Substring(0, 8).ToUpperInvariant();
                    if (shortCode == trimmedUpper)
                        return true;
                }

                return false;
            });
        }
    }
}
