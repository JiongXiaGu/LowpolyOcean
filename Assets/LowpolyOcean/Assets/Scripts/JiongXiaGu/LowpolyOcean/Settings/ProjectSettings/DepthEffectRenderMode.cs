namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// ocean refraction texture or use refracton depth texture render mode
    /// </summary>
    public enum DepthEffectRenderMode
    {
        /// <summary>
        /// render scenes with a separate camera, support transparent and geometric rendering queues, ocean can accept shadows
        /// </summary>
        Camera,

        /// <summary>
        /// use cached images, performance is better, but only support transparent rendering queue, because unity does not support transparent objects to accept shadows, so ocean can not accept shadows
        /// </summary>
        Buffer,
    }
}
