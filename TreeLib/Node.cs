using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeLib
{
    public class Node<KeyType, ValueType> : INode<KeyType, ValueType> where
        KeyType : IComparable<KeyType>
    {
        public KeyType Key { get; set; }
        public ValueType Payload { get; set; }

        public INode<KeyType, ValueType> LeftChild { get; set; }
        public INode<KeyType, ValueType> RightChild { get; set; }

 
        public bool IsRed { get; set; } 

        //isRed is implemented as a default (optional) parameter
        public Node(KeyType key, ValueType payload)
        {
            Key = key;
            Payload = payload;
            RightChild = null;
            LeftChild = null;
            IsRed = true;
        }

        public static int Height(INode<KeyType, ValueType> root)
        {
            //initial height initialization
            int height = -1; 

            if (root != null)
            {
                height = 1 + Math.Max(Node<KeyType, ValueType>.Height(root.LeftChild), Node<KeyType, ValueType>.Height(root.RightChild));
            }
            //Return Height of the tree
            return height;
        }

        public static void PreOrderTraversal(INode<KeyType, ValueType> root,
            ITraverse<KeyType, ValueType> traverser)
        {
            if (root != null)
            {
                traverser.ProcessNode(root.Key, root.Payload);//Process Node
                if (root.LeftChild != null)
                {
                    PostOrderTraversal(root.LeftChild, traverser);
                }
                if (root.RightChild != null)
                {
                    PostOrderTraversal(root.RightChild, traverser);
                }

            }
        }
        public static void ToString(INode<KeyType, ValueType> root, StringBuilder builder)
        {
            if (root != null)
            {
                builder.Append(root.Key.ToString()); //Append
                if (root.IsRed)
                    builder.Append("*");
                if (root.LeftChild != null || root.RightChild != null)
                {
                    builder.Append(" (");
                    ToString(root.LeftChild, builder);
                    builder.Append(") (");
                    ToString(root.RightChild, builder);
                    builder.Append(")");
                }
            }
            if (builder.Length == 0)
            {
                builder.Append("()");
            }
        }
        public static void PostOrderTraversal(INode<KeyType, ValueType> root,
           ITraverse<KeyType, ValueType> traverser)
        {
            if (root != null)
            {
                if (root.LeftChild != null)
                {
                    PostOrderTraversal(root.LeftChild, traverser);
                }
                if (root.RightChild != null)
                {
                    PostOrderTraversal(root.RightChild, traverser);
                }
                traverser.ProcessNode(root.Key, root.Payload);

            }
        }

        public static INode<KeyType, ValueType> RotateLeft(INode<KeyType, ValueType> root)
        {
            INode<KeyType, ValueType> newRoot = root.RightChild;
            root.RightChild = newRoot.LeftChild;
            newRoot.LeftChild = root;
            newRoot.IsRed = root.IsRed;
            root.IsRed = true;
            return newRoot;
        }
        //Assign new root correctly
        public static INode<KeyType, ValueType> RotateRight(INode<KeyType, ValueType> root)
        {
            INode<KeyType, ValueType> newRoot = root.LeftChild;
            root.LeftChild = newRoot.RightChild;
            newRoot.RightChild = root;
            newRoot.IsRed = root.IsRed;
            root.IsRed = true;
            return newRoot;
        }
        public static INode<KeyType, ValueType> RedBlackInsert(INode<KeyType, ValueType> root, KeyType key, ValueType payload)
        {
            if (root == null)
                root = new Node<KeyType, ValueType>(key, payload);
            int cmpVal = key.CompareTo(root.Key);
            if (cmpVal < 0)
            {// Compare to check which side to recurse
                root.LeftChild = RedBlackInsert(root.LeftChild, key, payload); // left child
            }
            else if (cmpVal > 0)
            {
                root.RightChild = RedBlackInsert(root.RightChild, key, payload); // right child
            }
       
            // Left leaning ensure nodes are not null
            if (root.LeftChild != null && root.RightChild != null)
            {
                if (root.RightChild.IsRed == true && root.LeftChild.IsRed == false)
                    root = RotateLeft(root);
            }
            if (root.LeftChild != null && root.LeftChild.LeftChild != null)
            {
                if (root.LeftChild.IsRed == true && root.LeftChild.LeftChild.IsRed == true) // case 2, 2 conecutive red links
                    root = RotateRight(root);
            }
            if (root.LeftChild != null && root.RightChild != null)
            {
                if (root.LeftChild.IsRed == true && root.RightChild.IsRed == true) //case 3 to red links to one node
                    switchColors(root);
            }
            if (root.LeftChild == null && root.RightChild != null)
            {
                    if (root.RightChild.IsRed == true)
                        root = RotateLeft(root);    
            }
                return root;
        }

        public static INode<KeyType, ValueType>
            InsertAtRoot(INode<KeyType, ValueType> root, KeyType key, ValueType value)
        { 
            if (root != null)
            {
                int comparison = key.CompareTo(root.Key); // Comparing keys
                if (comparison < 0)
                {
                    root.LeftChild = InsertAtRoot(root.LeftChild, key, value);
                    root = RotateRight(root);//rotateR
                }
                else
                {
                    root.RightChild = InsertAtRoot(root.RightChild, key, value);
                    root = RotateLeft(root);//RotateL
                }
            }
            else
            {
                root = new Node<KeyType, ValueType>(key, value);
            }
            return root;
        }

        public static void InOrderTraversal(INode<KeyType, ValueType> root,
            ITraverse<KeyType, ValueType> traverser)
        {

            if (root.LeftChild != null)
            {
                InOrderTraversal(root.LeftChild, traverser);
            }
            //Process Node
            traverser.ProcessNode(root.Key, root.Payload);

            if (root.RightChild != null)
            {
                InOrderTraversal(root.RightChild, traverser);
            }

        }
        public static void switchColors(INode<KeyType, ValueType> node)
        {
            node.IsRed = true;
            node.LeftChild.IsRed = false;
            node.RightChild.IsRed = false;
        }
      
        // Insert assumes that we already have a root node that we
        // are going to add a new node to.
        public static void Insert(INode<KeyType, ValueType> root,
                KeyType key, ValueType payload)
        {
            // Base case is the subtree we want to insert into is empty
            //    Add a new node to that root.
            // Base case is the keys match
            //    Change the payload
            // Otherwise
            //   Recursively call the insert function on the left or right subtree
            //   depending on whether the key is < or > the root key.

            // CompareTo returns 0 if ==, negative value if key<root.Key and positive value
            // otherwise
            int comparison = key.CompareTo(root.Key);

            if (comparison < 0)
            {
                // Need to insert into the left subtree

                if (root.LeftChild == null)
                {
                    // Base case - just insert new node here
                    root.LeftChild = new Node<KeyType, ValueType>(key, payload);
                }
                else
                {
                    // Recursively call the function on the left subtree
                    Insert(root.LeftChild, key, payload);
                }
            }
            else
            {
                if (comparison > 0)
                {
                    // Need to insert into the right subtree

                    if (root.RightChild == null)
                    {
                        // Base case - just insert new node here
                        root.RightChild = new Node<KeyType, ValueType>(key, payload);
                    }
                    else
                    {
                        // Recursively call the function on the right subtree
                        Insert(root.RightChild, key, payload);
                    }
                }
                else
                {
                    // The root key and the key being inserted are ==
                    root.Payload = payload;
                }
            }
        }
    }
}
