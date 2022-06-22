using Test.ViewModels;
using System.Net.Http;
using System.Text.Json;
using System.Text;
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
			var new_nota = new Nota() { ID = 0, Name = Input_noteTxt.Text, creation=DateTime.Now };
            var final_nota = SyncNotes(new_nota);
			if (final_nota == null)
				Error_label.Text = "Errore mente aggiungevo la nota";
			else
				_viewModel.AddNote(final_nota.ID, final_nota.Name);
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
		var endpoint = new Uri("https://localhost:5001/api/Registration/getNotes");
		var result = client.GetAsync(endpoint).Result;
		var json = result.Content.ReadAsStringAsync().Result;
		var response = JsonSerializer.Deserialize<List<Nota>>(json);

		foreach (var i in response)
			_viewModel.AddNote(i.ID, i.Name);
	}

	// aggiunge una nuova nota sul server
	private Nota SyncNotes(Nota newNote)
	{
		using var client = new HttpClient();
        var endpoint = new Uri("https://localhost:5001/api/Registration/addNote");
		var json = JsonSerializer.Serialize(newNote);
		var payload = new StringContent(json, Encoding.UTF8, "application/json");
		var request = client.PostAsync(endpoint, payload).Result;
		if (request.IsSuccessStatusCode)
        {
			var result = request.Content.ReadAsStringAsync().Result;
			return JsonSerializer.Deserialize<Nota>(result);
        }
        else
        {
            return null;
        }
    }

    private void Delete_Note(object sender, EventArgs e)
    {
		int id = Convert.ToInt32(SemanticProperties.GetDescription(sender as Button));
        using var client = new HttpClient();
        var endpoint = new Uri($"https://localhost:5001/api/Registration/deleteNote/{id}");
		var response = client.DeleteAsync(endpoint).Result;
		_viewModel.RemoveNote(id);
    }
}