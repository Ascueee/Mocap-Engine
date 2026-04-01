using Galaxy3D.ECS.Systems;
namespace Galaxy3D.ECS.Components;
/// <summary>
/// System ID represents the specfic archytype that the component represents example(0 = renderer)
/// Then the component gets stored in the arrays in that archytype.
/// </summary>
public interface IComponent
{
    public int componentID { get; set; }
    public bool isActive { get; set; }
}