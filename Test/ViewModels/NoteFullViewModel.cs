using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Input;
using NotesContracts;

namespace Test.ViewModels
{
    public class NoteFullViewModel : INotifyPropertyChanged, IDisposable
    {

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Generato alla pressione del tasto conferma in NoteFullView.xaml
        /// </summary>
        public EventHandler<(Note Note, bool IsNew)>? OnConfirm;

        /// <summary>
        /// Generato alla pressione del tasto cancella in NoteFullView.xaml
        /// </summary>
        public EventHandler<EventArgs>? OnExit;

        /// <summary>
        /// Generato quando si verifica un errore nell'insermento dati in NoteFullView.xaml
        /// </summary>
        public EventHandler<string> OnInsertError;

        private List<Categoria> _categories;

        private Note _currentNote;

        private Categoria _currentCategory;

        private readonly string api_key = "deeb9b705bfb40119fc4e494fb7b8529";

        public NoteFullViewModel()
        {
            _categories = new();
            GetCategories();
        }

        /// <summary>
        /// Nota che stò attualmente utilizzando 
        /// </summary>
        public Note CurrentNote 
        {
            get => _currentNote;
            set 
            {
                _currentNote = value;
                _currentCategory = _categories.FirstOrDefault(w => w.Id == _currentNote.CategoryID);
                OnPropertyChanged(nameof(CurrentCategory));
            }
        }

        public void SetCurrentCategory(Categoria newCategory)
        {
            CurrentCategory = newCategory;
            OnPropertyChanged(nameof(CurrentCategory));
        }


        /// <summary>
        /// 
        /// </summary>
        public Categoria CurrentCategory 
        { 
            get => _currentCategory; 
            set 
            {
                _currentCategory = value;
                if (value != null)
                    CurrentNote.CategoryID = value.Id;
            }
        }

        /// <summary>
        /// Determina se CurrentNote è nuova oppure viene aperta in modifica
        /// </summary>
        public bool NewNote { get; set; }

        public ObservableCollection<Categoria> Categories
        {
            get { return new(_categories);  }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Viene chiamato alla pressione del tasto "Conferma" nella NoteFullView e chiama la funzione AddNote()
        /// </summary>
        public ICommand ConfirmCommand => new Command<Note>((e) => AddNote(e));

        /// <summary>
        /// Viene chiamato alla pressione del tasto "Annulla" nella NoteFullView e chiama la funzione Exit()
        /// </summary>
        public ICommand ExitCommand => new Command(Exit);

        /// <summary>
        /// Genera l'evento OnConfirm ritornando una tupla contenente la nota create/modificata in base al valore di newNote
        /// </summary>
        private void AddNote(Note? e)
        {
            if (e == null)
            {
                OnInsertError?.Invoke(null, "Inserisci tutti i dati");
            }
            else
            {
                OnConfirm?.Invoke(null, ValueTuple.Create(CurrentNote, NewNote));
            }
        }

        /// <summary>
        /// Genera l'evento OnExit
        /// </summary>
        private void Exit()        
            => OnExit?.Invoke(null, null);

        
        private void GetCategories()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", api_key);
            HttpResponseMessage result = null;
            try
            {
                var endpoint = new Uri("https://mynotesapi.azure-api.net/v1/getCategories");
                result = client.GetAsync(endpoint).Result;
            }
            catch
            {
                OnInsertError?.Invoke(null, "Errore di connessione");
            }
            if (result.IsSuccessStatusCode)
            {
                var json = result.Content.ReadAsStringAsync().Result;
                var response = JsonSerializer.Deserialize<List<Categoria>>(json);

                _categories.Clear();
                _categories.AddRange(response);
            }
        }

        private void OnPropertyChanged(String property)
        {
            // il ? indica che fai l'Invoke solo quando PropertyChanged è != null
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

    }
}
