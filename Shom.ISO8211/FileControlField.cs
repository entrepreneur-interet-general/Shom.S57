using System;
using System.Collections.Generic;
using System.Text;

namespace Shom.ISO8211
{
    public class FileControlField : DataDescriptiveRecordField
    {
        private class TreeNode
        {
            public string Name;

            public TreeNode(string name)
            {
                this.Name = name;
            }

            public TreeNode(string name, string childName)
            {
                this.Name = name;
                this.Children.Add(new TreeNode(childName));
            }
            public List<TreeNode> Children = new List<TreeNode>();
        }

        //(special with tag 0..0)

        private readonly string _externalFileTitle;

        private readonly string _listOfFieldTagPairs;

        public FileControlField(string tag, ArraySegment<byte> fieldControls, string externalFileTitle, string listOfFieldTagPairs, int sizeOfTag)
            : base(tag, fieldControls)
        {
            _externalFileTitle = externalFileTitle;
            _listOfFieldTagPairs = listOfFieldTagPairs;

            ParseTagTree(sizeOfTag);
        }

        public string ExternalFileTitle
        {
            get { return _externalFileTitle; }
        }

        public string ListOfFieldTagPairs
        {
            get { return _listOfFieldTagPairs; }
        }

        private TreeNode _root = null;

        private void ParseTagTree(int sizeOfTag){
            //assume single root that is first node
            if (_listOfFieldTagPairs.Length % (sizeOfTag * 2) != 0)
            {
                throw new Exception("Expected list of tag pairs");
            }

            int currentIndex = 0;
            while (currentIndex < _listOfFieldTagPairs.Length)
            {
                string parent = _listOfFieldTagPairs.Substring(currentIndex, sizeOfTag);
                currentIndex += sizeOfTag;
                string child = _listOfFieldTagPairs.Substring(currentIndex, sizeOfTag);
                currentIndex += sizeOfTag;

                if (_root == null)
                {
                    _root = new TreeNode(parent, child);
                }
                else
                {
                    var node = FindNode(_root, parent);

                    if (node == null)
                    {
                        throw new NotImplementedException("Could not find parent node - multiple roots?");
                    } 
                    
                    node.Children.Add(new TreeNode(child));
                }
            }
        }

        private TreeNode FindNode(TreeNode node, string name)
        {
            if (node.Name == name)
            {
                return node;
            }

            foreach (var childNode in node.Children)
            {
                var foundNode = FindNode(childNode, name);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null;
        }

        private string OutputTree(TreeNode node, int level)
        {
            var sb = new StringBuilder();

            for (int i = 0; i <= level; i++){
                sb.Append("-");
            }

            sb.Append(node.Name + Environment.NewLine);

            foreach (var childNode in node.Children)
            {
                sb.Append(OutputTree(childNode, level + 1));
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return base.ToString() + "FileControl:" + ExternalFileTitle + Environment.NewLine + OutputTree(_root, 0) + Environment.NewLine;
        }
    }
}