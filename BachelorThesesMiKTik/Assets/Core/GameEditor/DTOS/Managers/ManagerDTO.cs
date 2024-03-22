using Assets.Core.GameEditor.DTOS;
using System;

[Serializable]
public class ManagerDTO
{
    public AssetSourceDTO[] Sources;

    public ManagerDTO() 
    {
        Sources = new AssetSourceDTO[0];
    }
    public ManagerDTO(AssetSourceDTO[] sources) 
    {
        Sources = sources;
    }
}
