using Cinemachine;

public interface ICameraSelector
{
    float SoftZoneWidth { get; }
    CinemachineFramingTransposer GetTransposer();
}