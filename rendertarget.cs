﻿using OpenTK.Graphics.OpenGL;

// based on http://www.opentk.com/doc/graphics/frame-buffer-objects

namespace Rasterization;

internal class RenderTarget
{
    private readonly int _colorTexture;
    private readonly uint _depthBuffer;
    private readonly uint _fbo;
    private readonly int _height;
    private readonly int _width;

    public RenderTarget(int screenWidth, int screenHeight)
    {
        _width = screenWidth;
        _height = screenHeight;
        // create color texture
        GL.GenTextures(1, out _colorTexture);
        GL.BindTexture(TextureTarget.Texture2D, _colorTexture);
        GL.ObjectLabel(ObjectLabelIdentifier.Texture, _colorTexture, -1, "colorTexture for RenderTarget");
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, _width, _height, 0, PixelFormat.Rgba,
            PixelType.UnsignedByte, IntPtr.Zero);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        // bind color and depth textures to fbo
        GL.GenFramebuffers(1, out _fbo);
        GL.GenRenderbuffers(1, out _depthBuffer);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        GL.ObjectLabel(ObjectLabelIdentifier.Framebuffer, _fbo, -1, "FBO for RenderTarget");
        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _colorTexture, 0);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthBuffer);
        GL.ObjectLabel(ObjectLabelIdentifier.Renderbuffer, _depthBuffer, -1, "depthBuffer for RenderTarget");
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage)All.DepthComponent24, _width,
            _height);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
            RenderbufferTarget.Renderbuffer, _depthBuffer);
        // test FBO integrity
        var untestedBoolean = CheckFboStatus();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // return to regular framebuffer
    }

    public int GetTextureId()
    {
        return _colorTexture;
    }

    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
    }

    public void Unbind()
    {
        // return to regular framebuffer
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    private bool CheckFboStatus()
    {
        switch (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer))
        {
            case FramebufferErrorCode.FramebufferComplete:
                // The framebuffer is complete and valid for rendering
                return true;
            case FramebufferErrorCode.FramebufferIncompleteAttachment:
                Console.WriteLine(
                    "FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
                break;
            case FramebufferErrorCode.FramebufferIncompleteMissingAttachment:
                Console.WriteLine("FBO: There are no attachments.");
                break;
            case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                Console.WriteLine(
                    "FBO: Attachments are of different size. All attachments must have the same width and height.");
                break;
            case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                Console.WriteLine(
                    "FBO: The color attachments have different format. All color attachments must have the same format.");
                break;
            case FramebufferErrorCode.FramebufferIncompleteDrawBuffer:
                Console.WriteLine(
                    "FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
                break;
            case FramebufferErrorCode.FramebufferIncompleteReadBuffer:
                Console.WriteLine(
                    "FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
                break;
            case FramebufferErrorCode.FramebufferUnsupported:
                Console.WriteLine("FBO: This particular FBO configuration is not supported by the implementation.");
                break;
            default:
                Console.WriteLine("FBO: Status unknown.");
                break;
        }

        return false;
    }
}