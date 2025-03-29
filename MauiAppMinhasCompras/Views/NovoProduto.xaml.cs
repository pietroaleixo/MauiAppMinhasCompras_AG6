using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class NovoProduto : ContentPage
{
    public NovoProduto()
    {
        InitializeComponent();
        // Inicializa o Picker com as categorias
        categoriaPicker.ItemsSource = new List<string> { "Alimentos", "Higiene", "Limpeza" };
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txt_descricao.Text) || string.IsNullOrEmpty(txt_quantidade.Text) || string.IsNullOrEmpty(txt_preco.Text) || categoriaPicker.SelectedItem == null)
            {
                await DisplayAlert("Atenção", "Preencha todos os campos e selecione uma categoria.", "OK");
                return;
            }

            Produto p = new Produto
            {
                Descricao = txt_descricao.Text,
                Quantidade = Convert.ToDouble(txt_quantidade.Text),
                Preco = Convert.ToDouble(txt_preco.Text),
                Categoria = categoriaPicker.SelectedItem.ToString() // Adiciona a categoria
            };

            await App.Db.Insert(p);
            await DisplayAlert("Sucesso!", "Registro Inserido", "OK");
            await Navigation.PopAsync();

        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}