using System.Runtime.CompilerServices;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Galaxy3D.SceneGraph.EngineEditorUI;

public class ImGuiController : IDisposable
{
    private bool _frameBegun;
    private int _vertexArray;
    private int _vertexBuffer;
    private int _indexBuffer;
    private int _fontTexture;
    private int _shader;
    private int _windowWidth;
    private int _windowHeight;

    public ImGuiController(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;

        IntPtr context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);
        
        var io = ImGui.GetIO();
        // REMOVED: DockingEnable flag removed for a standard floating window experience
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        
        io.Fonts.AddFontDefault();
        io.FontGlobalScale = 1f;

        CreateDeviceResources();
        SetPerFrameImGuiData(1f / 60f);
        
        
        _frameBegun = true;
    }

    private void CreateDeviceResources()
    {
        _vertexArray = GL.GenVertexArray();
        _vertexBuffer = GL.GenBuffer();
        _indexBuffer = GL.GenBuffer();

        string vertexSource = @"#version 330 core
            layout (location = 0) in vec2 Position;
            layout (location = 1) in vec2 UV;
            layout (location = 2) in vec4 Color;
            uniform mat4 ProjMtx;
            out vec2 Frag_UV;
            out vec4 Frag_Color;
            void main() {
                Frag_UV = UV;
                Frag_Color = Color;
                gl_Position = ProjMtx * vec4(Position.xy, 0, 1);
            }";

        string fragmentSource = @"#version 330 core
            in vec2 Frag_UV;
            in vec4 Frag_Color;
            uniform sampler2D Texture;
            layout (location = 0) out vec4 Out_Color;
            void main() {
                Out_Color = Frag_Color * texture(Texture, Frag_UV.st);
            }";

        _shader = CreateProgram(vertexSource, fragmentSource);

        var io = ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

        _fontTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        io.Fonts.SetTexID((IntPtr)_fontTexture);
        io.Fonts.ClearTexData();
    }
    
    public void Begin()
    {
        ImGui.NewFrame();
        _frameBegun = true;
    }

    public void Update(GameWindow wnd, float deltaSeconds)
    {
        SetPerFrameImGuiData(deltaSeconds);
        UpdateImGuiInput(wnd);
    }

    private void UpdateImGuiInput(GameWindow wnd)
    {
        var io = ImGui.GetIO();
        var mouseState = wnd.MouseState;

        io.AddKeyEvent(ImGuiKey.Delete, wnd.KeyboardState.IsKeyDown(Keys.Delete));
        io.AddKeyEvent(ImGuiKey.Backspace, wnd.KeyboardState.IsKeyDown(Keys.Backspace));
        io.AddKeyEvent(ImGuiKey.Enter, wnd.KeyboardState.IsKeyDown(Keys.Enter));
        io.AddKeyEvent(ImGuiKey.KeypadEnter, wnd.KeyboardState.IsKeyDown(Keys.KeyPadEnter));
        io.AddKeyEvent(ImGuiKey.Tab, wnd.KeyboardState.IsKeyDown(Keys.Tab));
        io.AddKeyEvent(ImGuiKey.LeftArrow, wnd.KeyboardState.IsKeyDown(Keys.Left));
        io.AddKeyEvent(ImGuiKey.RightArrow, wnd.KeyboardState.IsKeyDown(Keys.Right));
        io.AddKeyEvent(ImGuiKey.UpArrow, wnd.KeyboardState.IsKeyDown(Keys.Up));
        io.AddKeyEvent(ImGuiKey.DownArrow, wnd.KeyboardState.IsKeyDown(Keys.Down));
        
        // FIX: Map mouse position to the same units as io.DisplaySize
        io.MousePos = new System.Numerics.Vector2(mouseState.X, mouseState.Y);

        io.MouseDown[0] = mouseState.IsButtonDown(MouseButton.Left);
        io.MouseDown[1] = mouseState.IsButtonDown(MouseButton.Right);
        io.MouseDown[2] = mouseState.IsButtonDown(MouseButton.Middle);
    
        // FIX: Tell ImGui the ratio between pixels and logical units
        float scaleX = (float)wnd.FramebufferSize.X / wnd.Size.X;
        float scaleY = (float)wnd.FramebufferSize.Y / wnd.Size.Y;
        io.DisplayFramebufferScale = new System.Numerics.Vector2(scaleX, scaleY);

        var mouseCursor = ImGui.GetMouseCursor();
        wnd.Cursor = mouseCursor switch
        {
            ImGuiMouseCursor.ResizeNWSE => MouseCursor.ResizeNWSE,
            ImGuiMouseCursor.ResizeNS => MouseCursor.ResizeNS,
            ImGuiMouseCursor.ResizeEW => MouseCursor.ResizeEW,
            ImGuiMouseCursor.ResizeNESW => MouseCursor.ResizeNESW,
            ImGuiMouseCursor.Hand => MouseCursor.Hand,
            _ => MouseCursor.Default
        };
    }

    public unsafe void Render()
    {
        if (_frameBegun)
        {
            _frameBegun = false;
            ImGui.Render();
    
            ImDrawDataPtr drawData = ImGui.GetDrawData();
            if (drawData.NativePtr != null)
            {
                RenderImGuiDrawData(drawData);
            }
        }
    }

    private void RenderImGuiDrawData(ImDrawDataPtr drawData)
    {
        if (drawData.CmdListsCount == 0) return;

        // FIX: Viewport uses Pixels (Logical width * Scale)
        int fbWidth = (int)(_windowWidth * drawData.FramebufferScale.X);
        int fbHeight = (int)(_windowHeight * drawData.FramebufferScale.Y);
        GL.Viewport(0, 0, fbWidth, fbHeight);

        GL.Enable(EnableCap.Blend);
        GL.BlendEquation(BlendEquationMode.FuncAdd);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.ScissorTest);

        // FIX: Matrix uses Logical Units (_windowWidth) to match MousePos
        Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(0.0f, _windowWidth, _windowHeight, 0.0f, -1.0f, 1.0f);

        GL.UseProgram(_shader);
        GL.UniformMatrix4(GL.GetUniformLocation(_shader, "ProjMtx"), false, ref mvp);
        GL.BindVertexArray(_vertexArray);

        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[n];

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, cmdList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmdList.VtxBuffer.Data, BufferUsageHint.StreamDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, cmdList.IdxBuffer.Size * sizeof(ushort), cmdList.IdxBuffer.Data, BufferUsageHint.StreamDraw);

            int stride = Unsafe.SizeOf<ImDrawVert>();
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 8);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, stride, 16);

            for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
            {
                ImDrawCmdPtr cmd = cmdList.CmdBuffer[cmdi];
                
                // FIX: Scissor must use Physical Pixels
                int clipX = (int)(cmd.ClipRect.X * drawData.FramebufferScale.X);
                int clipY = (int)((_windowHeight - cmd.ClipRect.W) * drawData.FramebufferScale.Y);
                int clipW = (int)((cmd.ClipRect.Z - cmd.ClipRect.X) * drawData.FramebufferScale.X);
                int clipH = (int)((cmd.ClipRect.W - cmd.ClipRect.Y) * drawData.FramebufferScale.Y);

                GL.Scissor(clipX, clipY, clipW, clipH);

                GL.BindTexture(TextureTarget.Texture2D, (int)cmd.TextureId);
                GL.DrawElements(PrimitiveType.Triangles, (int)cmd.ElemCount, DrawElementsType.UnsignedShort, (int)cmd.IdxOffset * sizeof(ushort));
            }
        }
        GL.Disable(EnableCap.ScissorTest);
    }

    private void SetPerFrameImGuiData(float delta)
    {
        var io = ImGui.GetIO();
        io.DisplaySize = new System.Numerics.Vector2(_windowWidth, _windowHeight);
        io.DisplayFramebufferScale = new System.Numerics.Vector2(1.0f, 1.0f);
        io.DeltaTime = delta;
    }

    public void WindowResized(int w, int h) { _windowWidth = w; _windowHeight = h; }
    public void PressChar(char c) => ImGui.GetIO().AddInputCharacter(c);
    public void MouseScroll(Vector2 offset) => ImGui.GetIO().MouseWheel += offset.Y;

    private int CreateProgram(string v, string f)
    {
        int vs = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vs, v); GL.CompileShader(vs);
        int fs = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fs, f); GL.CompileShader(fs);
        int prog = GL.CreateProgram();
        GL.AttachShader(prog, vs); GL.AttachShader(prog, fs);
        GL.LinkProgram(prog);
        return prog;
    }

    public void Dispose() 
    { 
        GL.DeleteProgram(_shader); 
        GL.DeleteBuffer(_vertexBuffer);
        GL.DeleteBuffer(_indexBuffer);
        GL.DeleteVertexArray(_vertexArray);
    }
}