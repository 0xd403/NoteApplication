using System.Collections.ObjectModel;
using System.ComponentModel;
using Test.Models;

namespace Test.ViewModels;

public class NotaViewModel : INotifyPropertyChanged
{

    private List<Nota> _note;

    public event PropertyChangedEventHandler PropertyChanged;

    public NotaViewModel()
    {
        _note = new();
    }


    public ObservableCollection<Nota> Notes
    {
        get { return new(_note); }
    }

    public void AddNote(Nota new_note)
    {
        _note.Add(new_note);
        OnPropertyChanged(nameof(Notes));
    }

    public void RemoveNote(string id)
    {
        _note.Remove(_note.Find(i => i.ID == id));
        OnPropertyChanged(nameof(Notes));
    }

    private void OnPropertyChanged(String property)
    {
        // il ? indica che fai l'Invoke solo quando PropertyChanged è != null
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

}