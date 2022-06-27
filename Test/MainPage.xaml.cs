using Test.ViewModels;
using Test.Pages;
using Test.Models;
using System.Windows.Input;

namespace Test;

public partial class MainPage : ContentPage, IDisposable
{
	 
	// reference al wiewmodel che gestisce la CollectionView di note
	private NotaViewModel _viewModel;
    private NoteFullViewModel? _noteFullViewModel;

    public MainPage()
	{
		_viewModel = new();
        _viewModel.OnStartModify += ManageEditNote;
        _viewModel.OnStartDelete += ManageDeleteNote;

        _noteFullViewModel = new();
        _noteFullViewModel.OnConfirm += ManageOnConfirm;
        _noteFullViewModel.OnExit += ManageOnExit;


        // indica che nel Binding del file .xaml potrò utilizzare il tipo Nota
        BindingContext = _viewModel;

        InitializeComponent(); // disegno l'interfaccia

		_viewModel.LoadStartupNotes(); // carico le note dal server    
    }

    /// <summary>
    /// Chiamato alla ricezione dell'evento OnStartModify di NotaViewModel
    /// Aprirà la pagina NoteFullView in modifica passando la Note corrente
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ManageEditNote(object sender, Note e)
    {
        _noteFullViewModel.NewNote = false;
        _noteFullViewModel.CurrentNote = e;
        var page = new NoteFullView(_noteFullViewModel);
        await Navigation.PushAsync(page);
    }

    /// <summary>
    /// Chiamato alla ricezione dell'evento OnStartDelte di NotaViewModel
    /// Procederà ad eliminare la nota tramite il metodo RemoveNote di NotaViewModel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="id"></param>
    private async void ManageDeleteNote(object sender, string id)
    {
        _viewModel.RemoveNote(id);
        await DisplayAlert("Avviso", "Nota rimossa correttamente", "OK");
    }

    /// <summary>
    /// Chiamato alla ricezione dell'evento OnExit di NotaFullViewModel
    /// Chiuderà la pagina NoteFullView
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ManageOnExit(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    /// <summary>
    /// Chiamato alla ricezione dell'evento OnConfirm di NotaFullViewModel
    /// [Ovvero quando premo il tasto "Conferma" sulla pagina NotaFullView]
    /// Controllerà se la nota è nuova oppure modificata e chiamerà di conseguenza i metodi opportuni di NotaViewModel per aggiornare server e GUI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="values"></param>
    private async void ManageOnConfirm(object sender, (Note Note, bool IsNew) values)
    {
        string messaggio;
        if (values.IsNew)
        {
            messaggio = "inserita";
            _viewModel.AddNote(values.Note);
        }
        else
        {
            messaggio = "aggiornata";
            _viewModel.EditNote(values.Note.Id, values.Note.Text);
        }

        await Navigation.PopAsync();
        await DisplayAlert("Avviso", $"Nota {messaggio} correttamente", "OK");
    }

    public void Dispose()
    {
        _noteFullViewModel.OnConfirm -= ManageOnConfirm;
        _noteFullViewModel.OnExit -= ManageOnExit;
        _viewModel.OnStartModify -= ManageEditNote;
        GC.SuppressFinalize(this);
    }



    /// <summary>
    /// Permette di aggiungere una nuova nota.
    /// - Viene chiamato alla pressione del tasto + sulla MainPage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Event_addNote(object sender, EventArgs e)
    {
        _noteFullViewModel.NewNote = true;
        _noteFullViewModel.CurrentNote = new();
        var page = new NoteFullView(_noteFullViewModel);
        await Navigation.PushAsync(page);
    }
}