using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoggleChallenge.Models
{
    // Basic implementation of a trie using built-in Dictionary class
    public class TrieNode
    {
        public char currentLetter;
        public Dictionary<char, TrieNode> children;
        public string currentWord;
        public bool isWord;
        public TrieNode parent;

        public TrieNode()
        {
            children = new Dictionary<char, TrieNode>();
        }

        public TrieNode(TrieNode parentNode, char letter)
            : this()
        {
            currentLetter = letter;
            currentWord = parentNode == null ? letter.ToString() : parentNode.currentWord + letter;
            parent = parentNode;
            isWord = false;
        }

        public static TrieNode MakeTrieFromWordList(string[] words)
        {
            TrieNode root = new TrieNode();
            root.currentWord = "";
            root.currentLetter = '\0';
            root.parent = null;

            foreach (string word in words)
            {
                string lowercaseWord = word.ToLower();
                TrieNode current = root;
                for (int i = 0; i < lowercaseWord.Length; i++)
                {
                    char letter = lowercaseWord[i];
                    if (!current.children.ContainsKey(letter))
                    {
                        current.children[letter] = new TrieNode(current, letter);
                    }
                    current = current.children[letter];

                    if (i == lowercaseWord.Length - 1)
                    {
                        current.isWord = true;
                    }

                }
            }

            return root;
        }


        // Unused utility methods
        public void Print()
        {
            if (isWord)
            {
                Console.WriteLine(currentWord);
            }

            foreach (var key in children.Keys)
            {
                children[key].Print();
            }

        }

        public IEnumerable<TrieNode> EachNode()
        {
            Stack<TrieNode> nodeStack = new Stack<TrieNode>();
            nodeStack.Push(this);

            while (nodeStack.Count != 0)
            {
                TrieNode current = nodeStack.Pop();

                yield return current;

                foreach (TrieNode child in current.children.Values)
                {
                    nodeStack.Push(child);
                }
            }

        }
    }


    public class BoggleBoard
    {
        protected enum BoardState : byte { unassigned, taken }

        protected static TrieNode trie = TrieNode.MakeTrieFromWordList(System.IO.File.ReadAllLines(HttpContext.Current.Server.MapPath("~/words.txt")));

        protected HashSet<string> wordsInBoard;

        protected BoardState[,] boardState;

        protected char[,] board;

        protected int numRows;

        protected int numCols;

        protected BoggleBoard(char[,] board, int m, int n)
        {
            this.board = board;
            this.numRows = m;
            this.numCols = n;
        }

        // Uses a trie-assisted greedy search to find words in @board of dimensions @m x @n
        public static List<String> SolveBoggleBoard(char[,] board, int m, int n)
        {
            BoggleBoard boggle = new BoggleBoard(board, m, n);
            return boggle.Solve();
        }

        protected List<String> Solve()
        {
            wordsInBoard = new HashSet<string>();
            boardState = new BoardState[numRows, numCols];
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    boardState[i, j] = BoardState.unassigned;
                }
            }

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    char firstChar = board[i, j];
                    if (trie.children.ContainsKey(firstChar))
                    {
                        SolveHelper(trie.children[firstChar], i, j);
                    }
                }
            }

            return wordsInBoard.ToList();
        }

        // Searches for the next letter in a sequence ending at @i, @j using @node's children
        protected void SolveHelper(TrieNode node, int i, int j)
        {
            if (node.isWord && node.currentWord.Length > 3) wordsInBoard.Add(node.currentWord);
            if (node.children.Count == 0) return; // Return early if we reach a leaf node

            boardState[i, j] = BoardState.taken; // Mark this position as visited while we search for longer sequences

            foreach (var boardPos in EnumerateNeighbors(i, j))
            {
                if (boardPos.Item1 < 0 || boardPos.Item2 < 0 || boardPos.Item1 >= numRows || boardPos.Item2 >= numCols)
                {
                    continue; // Bound checking
                }

                if (node.children.ContainsKey(board[boardPos.Item1, boardPos.Item2]) && boardState[boardPos.Item1, boardPos.Item2] == BoardState.unassigned)
                {
                    // If this adjacent position forms part of a word and is not yet visited
                    char nextChar = board[boardPos.Item1, boardPos.Item2];
                    //SolveHelper(boardState, board, wordsInBoard, boardPos.Item1, boardPos.Item2, node.children[nextChar], m, n);
                    SolveHelper(node.children[nextChar], boardPos.Item1, boardPos.Item2);
                }
            }
            boardState[i, j] = BoardState.unassigned; // Once we're done with this position, mark it as unvisited
        }

        protected IEnumerable<Tuple<int, int>> EnumerateNeighbors(int i, int j)
        {
            yield return new Tuple<int, int>(i - 1, j - 1);
            yield return new Tuple<int, int>(i - 1, j);
            yield return new Tuple<int, int>(i, j - 1);
            yield return new Tuple<int, int>(i + 1, j + 1);
            yield return new Tuple<int, int>(i + 1, j);
            yield return new Tuple<int, int>(i, j + 1);
            yield return new Tuple<int, int>(i - 1, j + 1);
            yield return new Tuple<int, int>(i + 1, j - 1);
        }
    }
}