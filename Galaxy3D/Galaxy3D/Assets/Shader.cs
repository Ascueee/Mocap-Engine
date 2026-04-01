using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Galaxy3D.Assets;

public class Shader
{
    int handle;
    string vertexShaderPath;
    string fragmentShaderPath;
    string geometryShaderPath;

    public Shader(string vertexShaderPath, string fragmentShaderPath)
    {
        this.vertexShaderPath = vertexShaderPath;
        this.fragmentShaderPath = fragmentShaderPath;
    }
    
    public Shader(string vertexShaderPath, string fragmentShaderPath, string geometryShaderPath)
    {
        this.vertexShaderPath = vertexShaderPath;
        this.fragmentShaderPath = fragmentShaderPath;
        this.geometryShaderPath = geometryShaderPath;
    }
    
    public void Load()
    {
        int vertexShader;
        int fragmentShader;
        
        string vertexShaderSource = File.ReadAllText(vertexShaderPath);
        string fragmentShaderSource = File.ReadAllText(fragmentShaderPath);
        
        vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        
        fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        
        
        CompileShader(vertexShader);
        CompileShader(fragmentShader);
        
        handle = GL.CreateProgram();
        
        LinkProgram(vertexShader, fragmentShader);
        
    }
    
    public void LoadShaderWithGeometryShader()
    {
        int vertexShader;
        int fragmentShader;
        int geometryShader;
        
        //Reads the shader source code from the file paths
        string vertexShaderSource = File.ReadAllText(vertexShaderPath);
        string fragmentShaderSource = File.ReadAllText(fragmentShaderPath);
        string geometryShaderSource = File.ReadAllText(geometryShaderPath);


        
        vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        
        fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        
        geometryShader = GL.CreateShader(ShaderType.GeometryShader);
        GL.ShaderSource(geometryShader, geometryShaderSource);
        
        CompileShader(vertexShader);
        CompileShader(fragmentShader);
        CompileShader(geometryShader);
        
        handle = GL.CreateProgram();
        
        LinkProgram(vertexShader, fragmentShader, geometryShader);
    }
    
    public void Use()
    {
        GL.UseProgram(handle);
    }
    
    void LinkProgram(int vertexShader, int fragmentShader)
    {
        GL.AttachShader(handle, vertexShader);
        GL.AttachShader(handle, fragmentShader);
        
        GL.LinkProgram(handle);
        
        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int result);
        if (result == 0)
        {
            string infoLog = GL.GetProgramInfoLog(handle);
            Console.WriteLine(infoLog);
        }
        
        GL.DetachShader(handle, vertexShader);
        GL.DetachShader(handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }
    
        
    void LinkProgram(int vertexShader, int fragmentShader, int geometryShader)
    {
        GL.AttachShader(handle, vertexShader);
        GL.AttachShader(handle, fragmentShader);
        GL.AttachShader(handle, geometryShader);
        
        GL.LinkProgram(handle);
        
        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int result);
        if (result == 0)
        {
            string infoLog = GL.GetProgramInfoLog(handle);
            Console.WriteLine(infoLog);
        }
        
        GL.DetachShader(handle, vertexShader);
        GL.DetachShader(handle, fragmentShader);
        GL.DetachShader(handle, geometryShader);
        GL.DeleteShader(geometryShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
        
    }
    
    void CompileShader(int shader)
    {
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int result);

        if (result == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            Console.WriteLine(infoLog);
        }
        else
        {
            Console.Write(result);
            Console.WriteLine("Compiled Successfully");
        }
    }
    
    public void SetInt(string name, int val)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.Uniform1(location, val);
    }
    
    public void SetFloat(string name, float val)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.Uniform1(location, val);
    }
    
    public void SetVec3(string name, Vector3 val)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.Uniform3(location, val.X, val.Y, val.Z);
    }
    
    public void SetVec4(string name, Vector4 val)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.Uniform4(location, val.X, val.Y, val.Z, val.W);
    }
    
    public void SetMat4(string name, Matrix4 mat)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.UniformMatrix4(location, false, ref mat);
    }
    
    public void SetMat4Array(string name, Matrix4[] mats)
    {
        int location = GL.GetUniformLocation(handle, name);
        if (location == -1)
        {
            Console.WriteLine($"Warning: Uniform '{name}' not found!");
            return;
        }

        // Flatten the array into a contiguous float buffer
        float[] flat = new float[mats.Length * 16];
        for (int i = 0; i < mats.Length; i++)
        {
            flat[i * 16 + 0] = mats[i].M11;
            flat[i * 16 + 1] = mats[i].M12;
            flat[i * 16 + 2] = mats[i].M13;
            flat[i * 16 + 3] = mats[i].M14;

            flat[i * 16 + 4] = mats[i].M21;
            flat[i * 16 + 5] = mats[i].M22;
            flat[i * 16 + 6] = mats[i].M23;
            flat[i * 16 + 7] = mats[i].M24;

            flat[i * 16 + 8] = mats[i].M31;
            flat[i * 16 + 9] = mats[i].M32;
            flat[i * 16 + 10] = mats[i].M33;
            flat[i * 16 + 11] = mats[i].M34;

            flat[i * 16 + 12] = mats[i].M41;
            flat[i * 16 + 13] = mats[i].M42;
            flat[i * 16 + 14] = mats[i].M43;
            flat[i * 16 + 15] = mats[i].M44;
        }

        // Upload the entire array to the shader
        GL.UniformMatrix4(location, mats.Length, false, flat);
    }
}