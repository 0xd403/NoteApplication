using Test.ViewModels;
using Test.Models;

namespace Test;

public partial class MainPage : ContentPage {
	 
	// reference al wiewmodel che gestisce la CollectionView di note
	private NotaViewModel _viewModel;

    public MainPage()
	{
		_viewModel = new();

        // indica che nel Binding del file .xaml potrò utilizzare il tipo Nota
		BindingContext = _viewModel;

        InitializeComponent(); // disegno l'interfaccia

		_viewModel.LoadStartupNotes(); // carico le note dal server
    }

    /// <summary>
    /// Permette di aggiungere una nuova nota
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Event_addNote(object sender, EventArgs e)
    {
        string input_text = await DisplayPromptAsync("Inserisci", "Testo della nuova nota");

        if (!String.IsNullOrEmpty(input_text))
        {
            var new_nota = new Nota() { Name = input_text, creation = DateTime.Now };
            _viewModel.AddNote(new_nota);
            await DisplayAlert("Avviso", "Nota inserita correttamente", "OK");
        }
    }

    /// <summary>
    /// Permette di modificare il testo di una nota esistente dato il suo Id
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Edit_Note(object sender, EventArgs e)
    {
        string input_text = await DisplayPromptAsync("Inserisci", "Testo da modificare");

        if (!String.IsNullOrEmpty(input_text))
        {
            string id = (SemanticProperties.GetDescription(sender as ImageButton));
            _viewModel.EditNote(id, input_text);
            await DisplayAlert("Avviso", "Nota modificata correttamente", "OK");
        }
    }

    /// <summary>
    /// Permette la rimozione di una nota quando viene premuto il corrispondente bottone di eliminazione
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Delete_Note(object sender, EventArgs e)
    {
		string id = (SemanticProperties.GetDescription(sender as ImageButton));
        _viewModel.RemoveNote(id);
		await DisplayAlert("Avviso", "Nota rimossa correttamente", "OK");
    }

	
}