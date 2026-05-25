public interface ILoadingProgress
{
    bool IsInFade { get; }
    void Fade(bool val);
    void SetProgress(float progress);
}