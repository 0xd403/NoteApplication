using Test.ViewModels;
using System.Text;
using System.Text.Json;
using Test.Models;

namespace Test;

public partial class MainPage : ContentPage {
	 
	// lista di note (da usare nella collection view)
	private NotaViewModel _viewModel;

    public MainPage()
	{
		//var x = _note.OrderBy(o => o.ID); // ordino la lista _note in base all'id
		//var y = _note.Where(w => w.ID > 1); // preno tutti gli elementi della lista note aventi id > 1

		_viewModel = new();
		BindingContext = _viewModel; // assegno il contesto di questa pagina alla nota singola e per questo posso usare il binding nella label titolo nel file .xaml

		FetchNotes();
        InitializeComponent(); // disegno l'interfaccia
    }

	private void Event_addNote(object sender, EventArgs e)
	{
		Error_label.Text = "";
		if (String.IsNullOrEmpty(Input_noteTxt.Text))
		{
			Error_label.Text = "Testo non valido";
		}
		else
		{
			var id = _viewModel.Notes.Count;
			_viewModel.AddNote(id+1, Input_noteTxt.Text);
			SyncNotes(_viewModel.Notes[_viewModel.Notes.Count-1]);
        }
	}

    private void Event_clearForm(object sender, EventArgs e)
    {
        Input_noteTxt.Text = "";
    }

	// scarica le note dal server
	private void FetchNotes()
	{
		using var client = new HttpClient();
		var endpoint = new Uri("https://localhost:5001/api/Home/getNotes");
		var result = client.GetAsync(endpoint).Result;
		var json = result.Content.ReadAsStringAsync().Result;
		var response = JsonSerializer.Deserialize<List<Nota>>(json);

		foreach (var i in response)
			_viewModel.AddNote(i.ID, i.Name);
	}

	// aggiunge una nuova nota sul server
	private void SyncNotes(Nota newNote)
	{
		using var client = new HttpClient();
        var endpoint = new Uri("https://localhost:5001/api/Home/addNote");
		HttpContent content = new StringContent(JsonSerializer.Serialize(newNote), Encoding.UTF8, "application/json");
		var stream = content.ReadAsStream();
		var response = client.PutAsync(endpoint, content).Result;
		if (!response.IsSuccessStatusCode)
		{
			Error_label.Text = "Errore!" + response.StatusCode;
		}
    }
}