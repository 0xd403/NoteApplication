using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using Test.Models;

namespace Test.ViewModels;

public class NotaViewModel : INotifyPropertyChanged, IDisposable
{

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Viene chiamato quando l'utente inizia la modifica di una nota cliccando il tasto edit (icona matita) della MainPage
    /// </summary>
    public event EventHandler<Note>? OnStartModify;

    /// <summary>
    /// Viene chiamato quando l'utente inizia la rimozione di una nota cliccando il tasto remove (icona corce) della MainPage
    /// </summary>
    public EventHandler<string>? OnStartDelete;

    private List<Note> _note;

    private HttpClient client;

    private readonly string api_key = "deeb9b705bfb40119fc4e494fb7b8529";

    public NotaViewModel()
    {
        _note = new();
        client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", api_key);
    }

    public void Dispose()
    {
        client.Dispose(); // elimina l'HttpClient quando passa il garbage collector
        GC.SuppressFinalize(this);
    }


    public ObservableCollection<Note> Notes
    {
        get { return new(_note); }
    }

    /// <summary>
    /// Viene chiamato alla pressione del tasto edit (icona matita) nella MainPage e chiama l'evento OnStartModify
    /// </summary>
    public ICommand EditNoteCommand => new Command<Note>(
    (e) => OnStartModify?.Invoke(null, e));

    public ICommand DeleteNoteCommand => new Command<string>(
        (e) => OnStartDelete?.Invoke(null, e));

    /// <summary>
    /// Carica le note all'avvio dell'applicazione
    /// </summary>
    public void LoadStartupNotes()
    {
        var endpoint = new Uri("https://mynotesapi.azure-api.net/v1/getNotes");
        var result = client.GetAsync(endpoint).Result;

        if (result.IsSuccessStatusCode)
        {
            var json = result.Content.ReadAsStringAsync().Result;
            var response = JsonSerializer.Deserialize<List<Note>>(json);

            foreach (var i in response)
                _note.Add(i);

            OnPropertyChanged(nameof(Notes));
        }
        else
        {
            // Gestione errore
        }
    }

    /// <summary>
    /// Aggiunge una nuova nota e aggiorna la CollectionView
    /// </summary>
    /// <param name="new_note"></param>
    public void AddNote(Note new_note)
    {
        var endpoint = new Uri("https://mynotesapi.azure-api.net/v1/addNote");
        var json = JsonSerializer.Serialize(new_note);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");
        var request = client.PostAsync(endpoint, payload).Result;

        if (request.IsSuccessStatusCode)
        {
            _note.Add(new_note);
            OnPropertyChanged(nameof(Notes));
        }
        else
        {
            // gestione errore
        }  
    }

    /// <summary>
    /// Rimuove una nota e aggiorna il CollectionView
    /// </summary>
    /// <param name="id"></param>
    public void RemoveNote(string id)
    {
        var endpoint = new Uri($"https://mynotesapi.azure-api.net/v1/deleteNote/{id}");
        var response = client.DeleteAsync(endpoint).Result;

        if (response.IsSuccessStatusCode)
        {
            _note.Remove(_note.Find(i => i.Id == id));
            OnPropertyChanged(nameof(Notes));
        }
        else
        {
            // gestione errore
        }
    }

    /// <summary>
    /// Permette di modificare il testo di una nota esistente dato il suo Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="new_text"></param>
    public void EditNote(string id, string new_text)
    {
        var note = _note.Find(i => i.Id == id);

        if (note != null)
        {
            var endpoint = new Uri("https://mynotesapi.azure-api.net/v1/editNote");
            var json = JsonSerializer.Serialize<Note>(new Note() { Id=id, Text=new_text, CreatedDate=DateTime.Now});
            var payload = new StringContent(json, Encoding.UTF8, "application/json");
            var request = client.PostAsync(endpoint, payload).Result;

            if (request.IsSuccessStatusCode)
            {
                var reponse = request.Content.ReadAsStringAsync().Result;
                var deserialized_note = JsonSerializer.Deserialize<Note>(reponse);
                note.Id = deserialized_note.Id;
                note.Text = deserialized_note.Text;
                note.CreatedDate = deserialized_note.CreatedDate;
                OnPropertyChanged(nameof(Notes));
            }
            else
            {
                // Gestione errore
            }
            
        }
        else
        {
            // Nota inesistente
        }
    }

    private void OnPropertyChanged(String property)
    {
        // il ? indica che fai l'Invoke solo quando PropertyChanged è != null
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

}