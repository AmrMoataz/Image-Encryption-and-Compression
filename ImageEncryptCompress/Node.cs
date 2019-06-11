using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageEncryptCompress
{
    public class Node
    {
        public Node Parent { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public byte value { get; set; }
        public int Priority { get; set; }
        public string representation { get; set; }
        public int totalBits { get; set; }
    }
}
