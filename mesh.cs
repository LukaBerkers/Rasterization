using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Rasterization;

// Mesh and MeshLoader based on work by JTalton; https://web.archive.org/web/20160123042419/www.opentk.com/node/642
// Only triangles and quads with vertex positions, normals, and texture coordinates are supported
public class Mesh
{
    // data members
    public readonly string Filename; // for improved error reporting
    private int _quadBufferId; // element buffer object (EBO) for quad vertex indices (not in Modern OpenGL)
    private int _triangleBufferId; // element buffer object (EBO) for triangle vertex indices
    private int _vertexBufferId; // vertex buffer object (VBO) for vertex data
    public ObjQuad[]? Quads; // quads (4 indices into the vertices array)
    public ObjTriangle[]? Triangles; // triangles (3 indices into the vertices array)
    public ObjVertex[]? Vertices; // vertices (positions and normals in Object Space, and texture coordinates)

    // constructor
    public Mesh(string filename)
    {
        Filename = filename;
        MeshLoader loader = new();
        loader.Load(this, filename);
    }

    // initialization; called during first render
    public void Prepare()
    {
        if (_vertexBufferId == 0)
        {
            // generate interleaved vertex data array (uv/normal/position per vertex)
            GL.GenBuffers(1, out _vertexBufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);
            GL.ObjectLabel(ObjectLabelIdentifier.Buffer, _vertexBufferId, 8 + Filename.Length, "VBO for " + Filename);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices?.Length * Marshal.SizeOf(typeof(ObjVertex))),
                Vertices, BufferUsageHint.StaticDraw);

            // generate triangle index array
            GL.GenBuffers(1, out _triangleBufferId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _triangleBufferId);
            GL.ObjectLabel(ObjectLabelIdentifier.Buffer, _triangleBufferId, 17 + Filename.Length,
                "triangle EBO for " + Filename);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                (IntPtr)(Triangles?.Length * Marshal.SizeOf(typeof(ObjTriangle))), Triangles,
                BufferUsageHint.StaticDraw);

            if (OpenTKApp.AllowPrehistoricOpenGl)
            {
                // generate quad index array
                GL.GenBuffers(1, out _quadBufferId);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _quadBufferId);
                GL.ObjectLabel(ObjectLabelIdentifier.Buffer, _quadBufferId, 13 + Filename.Length,
                    "quad EBO for " + Filename);
                GL.BufferData(BufferTarget.ElementArrayBuffer,
                    (IntPtr)(Quads?.Length * Marshal.SizeOf(typeof(ObjQuad))), Quads, BufferUsageHint.StaticDraw);
            }
        }
    }

    // render the mesh using the supplied shader and matrix
    public void Render(Shader shader, Matrix4 objectToScreen, Matrix4 objectToWorld, Texture texture)
    {
        // on first run, prepare buffers
        Prepare();

        // enable shader
        GL.UseProgram(shader.ProgramId);

        // enable texture
        var textureLocation =
            GL.GetUniformLocation(shader.ProgramId, "diffuseTexture"); // get the location of the shader variable
        var textureUnit = 0; // choose a texture unit
        GL.Uniform1(textureLocation, textureUnit); // set the value of the shader variable to that texture unit
        GL.ActiveTexture(TextureUnit.Texture0 + textureUnit); // make that the active texture unit
        GL.BindTexture(TextureTarget.Texture2D,
            texture.Id); // bind the texture as a 2D image texture to the active texture unit

        // pass transforms to vertex shader
        GL.UniformMatrix4(shader.UniformObjectToScreen, false, ref objectToScreen);
        GL.UniformMatrix4(shader.UniformObjectToWorld, false, ref objectToWorld);
        GL.Uniform4(shader.UniformAmbientLight, MyApplication._light.lightColor);
        GL.Uniform3(shader.UniformLightPosition, MyApplication._light.lightPosition);
        GL.Uniform3(shader.UniformCameraPosition, MyApplication._camera.Position);

        // enable position, normal and uv attribute arrays corresponding to the shader "in" variables
        GL.EnableVertexAttribArray(shader.InVertexPositionObject);
        GL.EnableVertexAttribArray(shader.InVertexNormalObject);
        GL.EnableVertexAttribArray(shader.InVertexUv);

        // bind vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);

        // link vertex attributes to shader parameters 
        GL.VertexAttribPointer(shader.InVertexUv, 2, VertexAttribPointerType.Float, false, 32, 0);
        GL.VertexAttribPointer(shader.InVertexNormalObject, 3, VertexAttribPointerType.Float, true, 32, 2 * 4);
        GL.VertexAttribPointer(shader.InVertexPositionObject, 3, VertexAttribPointerType.Float, false, 32, 5 * 4);

        // bind triangle index data and render
        if (Triangles != null && Triangles.Length > 0)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _triangleBufferId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, Triangles.Length * 3);
        }

        // bind quad index data and render
        if (Quads != null && Quads.Length > 0)
        {
            if (OpenTKApp.AllowPrehistoricOpenGl)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _quadBufferId);
                GL.DrawArrays(PrimitiveType.Quads, 0, Quads.Length * 4);
            }

            throw new Exception("Quads not supported in Modern OpenGL");
        }

        // restore previous OpenGL state
        GL.UseProgram(0);
    }

    // layout of a single vertex
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjVertex
    {
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Vertex;
    }

    // layout of a single triangle
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjTriangle
    {
        public int Index0, Index1, Index2;
    }

    // layout of a single quad
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjQuad
    {
        public int Index0, Index1, Index2, Index3;
    }
}