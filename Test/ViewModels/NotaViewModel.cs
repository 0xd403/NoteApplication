using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using NotesContracts.NotesAPI.Requests;
using NotesContracts.NotesAPI.Responses;

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
    public EventHandler<Guid>? OnStartDelete;

    /// <summary>
    /// Generato quando si verificano errori di connessione/comunicazione con l'API
    /// </summary>
    public EventHandler<string>? OnConnectionError;

    private List<Note> _note;

    private HttpClient client;

    private List<Categoria> _categories;

    public NotaViewModel()
    {
        _note = new();
        client = new HttpClient();
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

    /// <summary>
    /// Viene chiamato alla pressione del tasto remove nella MainPage e chiama l'evento OnStartDelete
    /// </summary>
    public ICommand DeleteNoteCommand => new Command<Guid>(
        (e) => OnStartDelete?.Invoke(null, e));




    /// <summary>
    /// Carica le note all'avvio dell'applicazione
    /// </summary>
    public async void LoadStartupNotes()
    {
        HttpResponseMessage result = null;
        var token = await SecureStorage.GetAsync("JWT_token");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        try
        {
            var endpoint = new Uri("https://localhost:7170/getNotes");
            result = client.GetAsync(endpoint).Result;

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
                OnConnectionError?.Invoke(null, "Errore di connessione");
            }

        }
        catch
        {
            OnConnectionError?.Invoke(null, "Non sei connesso a internet");
        }
        
    }

    /// <summary>
    /// Aggiunge una nuova nota e aggiorna la CollectionView
    /// </summary>
    /// <param name="new_note"></param>
    public void AddNote(Note new_note)
    {
        HttpResponseMessage result = null;
        try
        {
            var note = new RequestAddNote()
            {
                CategoryID = new_note.CategoryID,
                Title = new_note.Title,
                Text = new_note.Text
            };
            var endpoint = new Uri("https://localhost:7170/addNote");
            var json = JsonSerializer.Serialize(new_note);
            var payload = new StringContent(json, Encoding.UTF8, "application/json");

            result = client.PostAsync(endpoint, payload).Result;
        }
        catch
        {
            OnConnectionError?.Invoke(null, "Non sei connesso a internet");
        }
        if (result.IsSuccessStatusCode)
        {
            _note.Add(new_note);
            OnPropertyChanged(nameof(Notes));
        }
        else
        {
            OnConnectionError?.Invoke(null, "Errore di connessione");
        }
    }

    /// <summary>
    /// Rimuove una nota e aggiorna il CollectionView
    /// </summary>
    /// <param name="id"></param>
    public void RemoveNote(Guid id)
    {
        HttpResponseMessage result = null;
        try
        {
            var endpoint = new Uri($"https://localhost:7170/deleteNote/{id}");
            result = client.DeleteAsync(endpoint).Result;
        }
        catch
        {
            OnConnectionError?.Invoke(null, "Non sei connesso a internet");
        }
        if (result.IsSuccessStatusCode)
        {
            _note.Remove(_note.Find(i => i.Id == id));
            OnPropertyChanged(nameof(Notes));
        }
        else
        {
            OnConnectionError?.Invoke(null, "Errore di connessione");
        }
    }

    /// <summary>
    /// Permette di modificare il testo di una nota esistente dato il suo Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="new_text"></param>
    public void EditNote(Note edited_note)
    {
        HttpResponseMessage result = null;
        Note note = null;
        try
        {
            note = _note.Find(i => i.Id == edited_note.Id);

            if (note != null)
            {
                var xx = new RequestEditNote()
                {
                    Id = edited_note.Id,
                    CategoryID = edited_note.CategoryID,
                    Title = edited_note.Title,
                    Text = edited_note.Text
                };
                var endpoint = new Uri("https://localhost:7170/editNote");
                var json = JsonSerializer.Serialize<Note>(edited_note);
                var payload = new StringContent(json, Encoding.UTF8, "application/json");
                result = client.PostAsync(endpoint, payload).Result;
            }
            else
            {
                OnConnectionError?.Invoke(null, "Nota inesistente");
            }
        }
        catch
        {
            OnConnectionError?.Invoke(null, "Non sei connesso a internet");
        }
        if (result.IsSuccessStatusCode)
        {
            var reponse = result.Content.ReadAsStringAsync().Result;
            var deserialized_note = JsonSerializer.Deserialize<Note>(reponse);
            note.Id = deserialized_note.Id;
            note.CategoryID = deserialized_note.CategoryID;
            note.Title = deserialized_note.Title;
            note.Text = deserialized_note.Text;
            OnPropertyChanged(nameof(Notes));
        }
        else
        {
            OnConnectionError?.Invoke(null, "Errore di connessione");
        }
    }

    private void OnPropertyChanged(String property)
    {
        // il ? indica che fai l'Invoke solo quando PropertyChanged è != null
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

}