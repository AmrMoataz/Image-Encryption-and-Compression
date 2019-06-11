using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Priority_Queue;
using ImageEncryptCompress;
//using PriorityQueueDemo;
///Algorithms Project


namespace ImageQuantization
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    
    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        public static List<Dictionary<byte, int>> histogramlist;
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;
            RGBPixel[,] Buffer = new RGBPixel[Height, Width];
            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }
        /// <summary>
        /// Take an Initial Seed As UnSigned Integer
        /// Then Xor The Most Significent Bit With The Bit In Tab Index
        /// Shift Result Left 1 bit Then Put The result of Xor in The Least Significent Bit
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="tab"></param>
        /// <param name="StrLen"></param>
        /// <returns></returns>
        public static uint LFSR(uint seed, int tab,int StrLen)
        {
            uint Tab_Bit, Most_Sig, XoRes,Alt_Seed;    
            for (int i = 0; i < 8; i++)
            {
                Alt_Seed = seed;
                Tab_Bit = ((Alt_Seed >>= tab) & 1);
                Alt_Seed = seed;
                Most_Sig = ((Alt_Seed >>= StrLen - 1) & 1);
                XoRes = Most_Sig ^ Tab_Bit;
                seed <<= 1;
                if (XoRes == 1)
                    seed++;
            }
            return seed;
        }
        /// <summary>
        /// Take the key "init seed after shifting" and color "Encrypt the Color"
        /// Xor The Least Significent 8 Bit With Color 
        /// The Return Result
        /// </summary>
        /// <param name="key"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static uint GetintenIstyChannel(uint key, byte color)
        {
            return (key & 255) ^ color;
        }
        /// <summary>
        /// Loop The 2d Matrix of photo Then Encrypt Each Byte With its Key
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="InitSeed"></param>
        /// <param name="TabIndx"></param>
        /// <returns></returns>
        public static RGBPixel[,] Process(RGBPixel[,] Buffer, string InitSeed, int TabIndx)
        {
            int X = Buffer.GetLength(0);
            int Y = Buffer.GetLength(1);
            int StrLen = InitSeed.Length;
            uint Key=Convert.ToUInt32(InitSeed,2);
            RGBPixel[,] Processed_Buffer = new RGBPixel[X, Y];
            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < Y; j++)
                {
                    Key = LFSR(Key, TabIndx, StrLen);
                    Processed_Buffer[i, j].red = Convert.ToByte(GetintenIstyChannel(Key, Buffer[i, j].red));
                    Key = LFSR(Key, TabIndx, StrLen);
                    Processed_Buffer[i, j].green = Convert.ToByte(GetintenIstyChannel(Key, Buffer[i, j].green));
                    Key = LFSR(Key, TabIndx, StrLen);
                    Processed_Buffer[i, j].blue = Convert.ToByte(GetintenIstyChannel(Key, Buffer[i, j].blue));
                }
            }

            return Processed_Buffer;
        }
        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }


        /// <summary>
        /// get the three histograms for red, blue and green and put it in dictionaries
        /// store the histograms in queues
        /// </summary>
        /// <param name="ImageMatrix"></param>
        /// <returns>list of three histograms</returns>
        public static List<SimplePriorityQueue<Node, int>> getHistogram(RGBPixel[,] ImageMatrix)
        {

            List<SimplePriorityQueue<Node, int>> histrogramlist = new List<SimplePriorityQueue<Node, int>>();

            //Dictionaries
            Dictionary<byte, int> RedHistogram = new Dictionary<byte, int>();
            Dictionary<byte, int> blueHistogram = new Dictionary<byte, int>();
            Dictionary<byte, int> GreenHistogram = new Dictionary<byte, int>();

            //Queues
            SimplePriorityQueue<Node, int> _RedHistogram = new SimplePriorityQueue<Node, int>();
            SimplePriorityQueue<Node, int> _BlueHistogram = new SimplePriorityQueue<Node, int>();
            SimplePriorityQueue<Node, int> _GreenHistogram = new SimplePriorityQueue<Node, int>();



            //getting histograms from the matrix and store it in dictionaries
            int X = ImageMatrix.GetLength(0);
            int Y = ImageMatrix.GetLength(1);


            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < Y; j++)
                {
                    // Red
                    byte redCol = ImageMatrix[i, j].red;
                    if (RedHistogram.ContainsKey(redCol))
                    {
                        RedHistogram[redCol] += 1;
                    }

                    else
                        RedHistogram.Add(redCol, 1);

                    // Blue
                    byte blueCol = ImageMatrix[i, j].blue;
                    if (blueHistogram.ContainsKey(blueCol))
                    {
                        blueHistogram[blueCol] += 1;
                    }

                    else
                        blueHistogram.Add(blueCol, 1);

                    // Green
                    byte GreenCol = ImageMatrix[i, j].green;
                    if (GreenHistogram.ContainsKey(GreenCol))
                    {
                        GreenHistogram[GreenCol] += 1;
                    }
                    else
                        GreenHistogram.Add(GreenCol, 1);
                }
            }

            // puting the histograms in queues

            foreach (var node in RedHistogram)
            {
                Node colNode = new Node();
                colNode.value = node.Key;
                colNode.Priority = node.Value;
                _RedHistogram.Enqueue(colNode, colNode.Priority);
            }

            foreach (var node in blueHistogram)
            {
                Node colNode = new Node();
                colNode.value = node.Key;
                colNode.Priority = node.Value;
                _BlueHistogram.Enqueue(colNode, colNode.Priority);
            }

            foreach (var node in GreenHistogram)
            {
                Node colNode = new Node();
                colNode.value = node.Key;
                colNode.Priority = node.Value;
                _GreenHistogram.Enqueue(colNode, colNode.Priority);
            }


            histrogramlist.Add(_RedHistogram);
            histrogramlist.Add(_BlueHistogram);
            histrogramlist.Add(_GreenHistogram);

            return histrogramlist;
        }


        


        /// <summary>
        /// take the three Queues and build the huffman tree
        /// </summary>
        /// <param name="QueuesList"></param>
        /// <returns></returns>
        public static void BuildHuffmanTree(List<SimplePriorityQueue<Node, int>> QueuesList)
        {
            Node left;
            Node right;
            Node Parent;          

            for (int i = 0; i < QueuesList.Count; i++)
            {
                while(QueuesList[i].Count != 1)
                {
                    left = new Node();
                    left = QueuesList[i].Dequeue();
                    right = new Node();
                    right = QueuesList[i].Dequeue();

                    //Parent.value = (byte)(left.value + right.value);
                    Parent = new Node();
                    Parent.Priority = left.Priority + right.Priority;
                    Parent.Left = left;
                    Parent.Right = right;
                    QueuesList[i].Enqueue(Parent, Parent.Priority);
                }
            }

            string rep = null;
            int redTotal = 0;
            int blueTotal = 0;
            int greenTotal = 0;
            float compressionOutput;

            List<Node> redNodes = new List<Node>();
            List<Node> blueNodes = new List<Node>();
            List<Node> greenNodes = new List<Node>();

            EncodeColors(QueuesList[0].First, rep, ref redTotal, ref redNodes);
            EncodeColors(QueuesList[1].First, rep, ref blueTotal, ref blueNodes);
            EncodeColors(QueuesList[2].First, rep, ref greenTotal, ref greenNodes);

            foreach(Node node in redNodes)
            {
                //put in file
                /*
                * each node contain:
                * color (as value)
                * frequency (as priority)
                * huffman representation (as representation)
                * total bits
                */
            }

            foreach (Node node in blueNodes)
            {
                //put in file
                /*
                * each node contain:
                * color (as value)
                * frequency (as priority)
                * huffman representation (as representation)
                * total bits
                */
            }

            foreach (Node node in greenNodes)
            {
                //put in file
                /*
                 * each node contain:
                 * color (as value)
                 * frequency (as priority)
                 * huffman representation (as representation)
                 * total bits
                 */
            }

            // the final output -- this also need to be saved in a file
            int total = redTotal + blueTotal + greenTotal;
            compressionOutput = total / 8; 
        }

        /// <summary>
        /// take the root node and construct the huffman representation for each value
        /// </summary>
        /// <param name="root"></param>
        /// <param name="rep"></param>
        /// <param name="total"></param>
        /// <param name="nodeList"></param>
        private static void EncodeColors(Node root, string rep, ref int total, ref List<Node> nodeList)
        {
            //string temprep = null;
            string finalRep = rep;
            if (root.Left != null)
            {
                //temprep += rep;
                rep += 1;
                EncodeColors(root.Left, rep, ref total, ref nodeList);
                rep = finalRep;
            }
            if (root.Right != null)
            {
                //rep += temprep;
                rep += 0;
                EncodeColors(root.Right, rep, ref total, ref nodeList);
                rep = finalRep;
            }
            if (root.Left == null && root.Right == null)
            {
                root.representation = finalRep;
                int numOfbits = root.representation.Length;
                root.totalBits = numOfbits * root.Priority;
                total += root.totalBits;
                nodeList.Add(root);
            }
            
        }
    }

    
}
