using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Rasterization
{
    public class Node
    {
        Matrix4 ObjectToParent;
        Mesh? mesh;
        Shader shade;
        Texture? texture;
        //Node world;
        List<Node> children;
        public Node(Matrix4 ObjectToParent, Mesh? mesh, Shader shade, Texture? texture, List<Node> children)
        {
            this.ObjectToParent = ObjectToParent;
            this.mesh = mesh;
            this.shade = shade;
            this.texture = texture;
            this.children = children;
        }

        public void render(Matrix4 WorldToScreen, Matrix4 ParentToWorld)
        {
            Matrix4 ObjectToWorld = ObjectToParent * ParentToWorld;
            Matrix4 ObjectToScreen = ObjectToWorld * WorldToScreen;
            if(mesh != null || texture != null) // null error
            {
                mesh.Render(shade, ObjectToScreen, ObjectToWorld, texture);
            }
            

            foreach(var child in children)
            {
                child.render(ObjectToScreen, ObjectToWorld);
            }
        }
    }
}
