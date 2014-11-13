using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BoggleChallenge.Models
{
    public class Node
    {
        public Node next;
        public Node rand;
        public char tag;

        public Node(char t)
            : this()
        {
            tag = t;
        }

        public Node()
        {
            next = null;
            rand = null;
        }

        public override string ToString()
        {
            Node current = this;
            StringBuilder sb = new StringBuilder();

            string prefix = "";
            while (current != null)
            {
                sb.Append(prefix + current.tag + " (Rand: " + current.rand.tag + ", uniqueid: " + current.GetHashCode() + ")");
                current = current.next;
                prefix = " -> ";
            }

            return sb.ToString();
        }

        public static Node MakeRandomLinkedList(int size)
        {
            char currentTag = 'a';
            Node head = new Node(currentTag);
            Node[] nodeList = new Node[size];
            nodeList[0] = head;

            // Generate |size| nodes
            for (int i = 1; i < size; i++)
            {
                nodeList[i] = new Node(++currentTag);
                nodeList[i - 1].next = nodeList[i];
            }

            // Hook up random ptrs
            Random randGen = new Random();
            for (int i = 0; i < size; i++)
            {
                nodeList[i].rand = nodeList[randGen.Next(0, size)];
            }

            return head;
        }

        public static Node DuplicateList(Node head)
        {
            Node current = head;

            while (current != null)
            {
                // Insert copy between original nodes
                Node currentCopy = new Node(current.tag);
                currentCopy.next = current.next;
                current.next = currentCopy;

                current = currentCopy.next;
            }

            current = head;

            while (current != null)
            {
                // Hook up random ptrs
                current.next.rand = current.rand.next;
                current = current.next.next;
            }

            current = head;
            Node copyHead = head.next;

            while (current != null)
            {
                // Untangle the lists
                Node original = current; // For readability
                Node copy = current.next;

                current = current.next.next; // For the next iteration

                original.next = copy.next;
                if (copy.next != null) copy.next = copy.next.next;
            }

            return copyHead;
        }
    }
}