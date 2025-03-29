using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class EditarProduto : ContentPage
{
    public EditarProduto()
    {
        InitializeComponent();
        // Inicializa o Picker com as categorias
        categoriaPicker.ItemsSource = new List<string> { "Alimentos", "Higiene", "Limpeza" };
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Produto produto_anexado = BindingContext as Produto;

            if (string.IsNullOrEmpty(txt_descricao.Text) || string.IsNullOrEmpty(txt_quantidade.Text) || string.IsNullOrEmpty(txt_preco.Text) || categoriaPicker.SelectedItem == null)
            {
                await DisplayAlert("Atenção", "Preencha todos os campos e selecione uma categoria.", "OK");
                return;
            }

            Produto p = new Produto
            {
                Id = produto_anexado.Id,
                Descricao = txt_descricao.Text,
                Quantidade = Convert.ToDouble(txt_quantidade.Text),
                Preco = Convert.ToDouble(txt_preco.Text),
                Categoria = categoriaPicker.SelectedItem.ToString() // Atualiza a categoria
            };

            await App.Db.Update(p);
            await DisplayAlert("Sucesso!", "Registro Atualizado", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Configura o BindingContext e seleciona a categoria no Picker
        if (BindingContext is Produto produto)
        {
            categoriaPicker.SelectedItem = produto.Categoria;
        }
    }
}