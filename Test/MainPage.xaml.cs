using Test.ViewModels;

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
			_viewModel.AddNote(Input_noteTxt.Text);
        }
	}

    private void Event_clearForm(object sender, EventArgs e)
    {
        Input_noteTxt.Text = "";
    }
}