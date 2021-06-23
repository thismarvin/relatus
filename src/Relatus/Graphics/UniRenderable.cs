using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public class UniRenderable
    {
        public RenderOptions RenderOptions;

        public MeshData Mesh;
        public Transform Transform;
        public Color Tint;
        public Material Material;

        private static Effect uniShader;
        private static GraphicsDevice graphicsDevice;

        static UniRenderable()
        {
            uniShader = AssetManager.GetEffect("Relatus_3DEffect");
            graphicsDevice = Engine.Graphics.GraphicsDevice;
        }

        public void Draw(Camera camera)
        {
            uniShader.Parameters["WVP"].SetValue(camera.WVP);

            uniShader.Parameters["Model"].SetValue(Transform.Matrix);
            uniShader.Parameters["Normal"].SetValue(Transform.Normal);
            uniShader.Parameters["Tint"].SetValue(Tint.ToVector4());
            uniShader.Parameters["Shininess"].SetValue(Material.Shininess);

            uniShader.Parameters["CameraPosition"].SetValue(camera.Position);

            uniShader.Parameters["EnvironmentColor"].SetValue((ColorExt.CreateFromHex("#514493") * 0.5f).ToVector3());

            uniShader.Parameters["DiffuseMap"].SetValue(Material.DiffuseMap);
            uniShader.Parameters["NormalMap"].SetValue(Material.NormalMap);
            uniShader.Parameters["SpecularMap"].SetValue(Material.SpecularMap);

            uniShader.Parameters["LightOneDirection"].SetValue(new Vector3(1, -2, 3));
            uniShader.Parameters["LightOneDiffuse"].SetValue((ColorExt.CreateFromHex("#ff9000") * 0.6f).ToVector3());
            uniShader.Parameters["LightOneSpecular"].SetValue((ColorExt.CreateFromHex("#546aff") * 0.4f).ToVector3());

            uniShader.Parameters["LightTwoDirection"].SetValue(new Vector3(-3, 1, -2));
            uniShader.Parameters["LightTwoDiffuse"].SetValue((ColorExt.CreateFromHex("#546aff") * 0.6f).ToVector3());
            uniShader.Parameters["LightTwoSpecular"].SetValue((ColorExt.CreateFromHex("#ff9000") * 0.4f).ToVector3());

            graphicsDevice.SetVertexBuffer(Mesh.VertexBuffer);
            graphicsDevice.Indices = Mesh.IndexBuffer;
            uniShader.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Mesh.TotalTriangles);
        }
    }
}
