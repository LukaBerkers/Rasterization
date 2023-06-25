using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization
{
    public class Light
    {
        public Vector3 lightPosition;
        public Vector4 lightColor;
        public Light(Vector3 position, Vector4 color) 
        {
            this.lightPosition = position;
            this.lightColor = color;
        }
    }
}
