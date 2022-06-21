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
        _note = new()
        {
            new() {ID = 1, Name = "nota di prova" }
        };
    }


    public ObservableCollection<Nota> Notes
    {
        get { return new(_note); }
    }

    public void AddNote(string testo)
    {
        var new_id = _note.Count + 1;
        _note.Add(new Nota() { ID = new_id, Name = testo });
        OnPropertyChanged(nameof(Notes));
    }

    private void OnPropertyChanged(String property)
    {
        // il ? indica che fai l'Invoke solo quando PropertyChanged è != null
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

}