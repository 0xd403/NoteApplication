using System.Windows.Input;
using Test.Models;

namespace Test.ViewModels
{
    public class NoteFullViewModel : IDisposable
    {        
        public EventHandler<(Note Note, bool IsNew)>? OnConfirm;
        public EventHandler<EventArgs>? OnExit;

        public NoteFullViewModel()
        {
        }

        /// <summary>
        /// Nota che stò attualmente utilizzando 
        /// </summary>
        public Note CurrentNote { get; set; }

        /// <summary>
        /// Determina se CurrentNote è nuova oppure viene aperta in modifica
        /// </summary>
        public bool NewNote { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Viene chiamato alla pressione del tasto "Conferma" nella NoteFullView e chiama la funzione AddNote()
        /// </summary>
        public ICommand ConfirmCommand => new Command(AddNote);

        /// <summary>
        /// Viene chiamato alla pressione del tasto "Annulla" nella NoteFullView e chiama la funzione Exit()
        /// </summary>
        public ICommand ExitCommand => new Command(Exit);

        /// <summary>
        /// Genera l'evento OnConfirm ritornando una tupla contenente la nota create/modificata in base al valore di newNote
        /// </summary>
        private void AddNote()
            => OnConfirm?.Invoke(null, ValueTuple.Create(CurrentNote, NewNote));        

        /// <summary>
        /// Genera l'evento OnExit
        /// </summary>
        private void Exit()        
            => OnExit?.Invoke(null, null);
        
    }
}
