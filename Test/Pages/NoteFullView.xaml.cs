using Test.Models;
using Test.ViewModels;

namespace Test.Pages;

public partial class NoteFullView : ContentPage
{

	/// <summary>
	/// Reference al viewmodel responsabile di questa pagina
	/// </summary>
	private NoteFullViewModel _viewModel;

	/// <summary>
	/// Costruttore della classe che si occupa di inizializzare il viewModel e il BindingContext
	/// </summary>
	/// <param name="viewModel">
	///	L'istanza di tipo viewModel viene creata dalla MainPage e viene passata a questa pagina durante la creazione
	/// </param>
	public NoteFullView(NoteFullViewModel viewModel)
	{
		_viewModel = viewModel;
		BindingContext = _viewModel;

		InitializeComponent();
	}
}