using Test.ViewModels;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Test.Models;
using Google.Apis.Auth.OAuth2;

namespace Test;

public partial class MainPage : ContentPage {
	 
	// lista di note (da usare nella collection view)
	private NotaViewModel _viewModel;

	private readonly string api_key = "deeb9b705bfb40119fc4e494fb7b8529";

    public MainPage()
	{
		//var x = _note.OrderBy(o => o.ID); // ordino la lista _note in base all'id
		//var y = _note.Where(w => w.ID > 1); // preno tutti gli elementi della lista note aventi id > 1

		_viewModel = new();
		BindingContext = _viewModel; // assegno il contesto di questa pagina alla nota singola e per questo posso usare il binding nella label titolo nel file .xaml

        InitializeComponent(); // disegno l'interfaccia
		if (!FetchNotes())
			Error_label.Text = "Impossibile connettersi al server!";
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
			var new_nota = new Nota() { Name = Input_noteTxt.Text, creation=DateTime.Now };
            var final_nota = SyncNotes(new_nota);
			if (final_nota == null)
				Error_label.Text = "Errore mente aggiungevo la nota";
			else
				_viewModel.AddNote(new_nota);
        }
	}

    private void Event_clearForm(object sender, EventArgs e)
    {
        Input_noteTxt.Text = "";
    }

	// scarica le note dal server
	private bool FetchNotes()
	{
		using var client = new HttpClient();
		client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", api_key);
		var endpoint = new Uri("https://mynotesapi.azure-api.net/v1/getNotes");
		try
		{
            var result = client.GetAsync(endpoint).Result;
            var json = result.Content.ReadAsStringAsync().Result;
            var response = JsonSerializer.Deserialize<List<Nota>>(json);

            foreach (var i in response)
                _viewModel.AddNote(i);
        }
		catch
		{
			return false;
		}
		return true;
	}

	// aggiunge una nuova nota sul server
	private Nota SyncNotes(Nota newNote)
	{
		using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", api_key);
        var endpoint = new Uri("https://mynotesapi.azure-api.net/v1/addNote");
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
		string id = (SemanticProperties.GetDescription(sender as Button));
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", api_key);
        var endpoint = new Uri($"https://mynotesapi.azure-api.net/v1/deleteNote/{id}");
		var response = client.DeleteAsync(endpoint).Result;
		_viewModel.RemoveNote(id);
    }
}