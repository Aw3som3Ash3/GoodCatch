public interface IInteractable
{
    public bool IsInteractable { get;}
    public bool Interactable()
    {
        return IsInteractable;

    }
    public bool Interact();

    public string StationName { get; }
}
