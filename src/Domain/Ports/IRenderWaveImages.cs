namespace Domain.Ports
{
    using System.Drawing;

    public interface IRenderWaveImages
    {
        Image Render(string filename);
    }
}