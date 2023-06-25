using OpenTK.Graphics.OpenGL;

namespace Rasterization;

public class Shader
{
    public int InVertexNormalObject;
    public int InVertexPositionObject;

    public int InVertexUv;

    // data members
    public int ProgramId, VsId, FsId;
    public int UniformObjectToScreen;
    public int UniformObjectToWorld;
    public int UniformAmbientLight;
    public int UniformLightPosition;
    public int UniformCameraPosition;

    // constructor
    public Shader(string vertexShader, string fragmentShader)
    {
        // compile shaders
        ProgramId = GL.CreateProgram();
        GL.ObjectLabel(ObjectLabelIdentifier.Program, ProgramId, -1, vertexShader + " + " + fragmentShader);
        Load(vertexShader, ShaderType.VertexShader, ProgramId, out VsId);
        Load(fragmentShader, ShaderType.FragmentShader, ProgramId, out FsId);
        GL.LinkProgram(ProgramId);
        var infoLog = GL.GetProgramInfoLog(ProgramId);
        if (infoLog.Length != 0) Console.WriteLine(infoLog);

        // get locations of shader parameters
        InVertexPositionObject = GL.GetAttribLocation(ProgramId, "vertexPositionObject");
        InVertexNormalObject = GL.GetAttribLocation(ProgramId, "vertexNormalObject");
        InVertexUv = GL.GetAttribLocation(ProgramId, "vertexUV");
        UniformObjectToScreen = GL.GetUniformLocation(ProgramId, "objectToScreen");
        UniformObjectToWorld = GL.GetUniformLocation(ProgramId, "objectToWorld");
        UniformAmbientLight = GL.GetUniformLocation(ProgramId, "lightColor");
        UniformLightPosition = GL.GetUniformLocation(ProgramId, "lightPosition");
        UniformCameraPosition = GL.GetUniformLocation(ProgramId, "cameraPosition");
    }

    // loading shaders
    private void Load(string filename, ShaderType type, int program, out int id)
    {
        // source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
        id = GL.CreateShader(type);
        GL.ObjectLabel(ObjectLabelIdentifier.Shader, id, -1, filename);
        using (var sr = new StreamReader(filename))
        {
            GL.ShaderSource(id, sr.ReadToEnd());
        }

        GL.CompileShader(id);
        GL.AttachShader(program, id);
        var infoLog = GL.GetShaderInfoLog(id);
        if (infoLog.Length != 0) Console.WriteLine(infoLog);
    }
}