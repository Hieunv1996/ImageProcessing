using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcesing
{
    public class Kernel
    {
        public int[] Matrix { get; set; }
        public int Size { get; set; }

        public Kernel(int[] a, int size)
        {
            Matrix = a;
            Size = size;
        }
    }
}
