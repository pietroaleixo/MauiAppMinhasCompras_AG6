using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
    {
        InitializeComponent();

        lst_produtos.ItemsSource = lista;
        categoriaPicker.ItemsSource = new List<string> { "Todas", "Alimentos", "Higiene", "Limpeza" };
        categoriaPicker.SelectedItem = "Todas";
    }

    protected async override void OnAppearing()
    {
        await AtualizarListaProdutos();
    }

    private async Task AtualizarListaProdutos()
    {
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
            AjustarIDs();
            ExibirRelatorioGastosPorCategoria();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void AjustarIDs()
    {
        for (int i = 0; i < lista.Count; i++)
        {
            lista[i].Id = i + 1;
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        await FiltrarProdutos();
    }

    private async void categoriaPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        await FiltrarProdutos();
    }

    private async Task FiltrarProdutos()
    {
        try
        {
            string q = txt_search.Text;
            string categoriaSelecionada = categoriaPicker.SelectedItem.ToString();

            lst_produtos.IsRefreshing = true;

            lista.Clear();

            List<Produto> tmp = await App.Db.Search(q);

            if (categoriaSelecionada != "Todas")
            {
                tmp = tmp.Where(p => p.Categoria == categoriaSelecionada).ToList();
            }

            tmp.ForEach(i => lista.Add(i));
            AjustarIDs();
            ExibirRelatorioGastosPorCategoria();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        string msg = $"O total é {soma:C}";

        DisplayAlert("Total dos Produtos", msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem;

            Produto p = selecionado.BindingContext as Produto;

            bool confirm = await DisplayAlert(
                "Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
                AjustarIDs();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;

            if (p != null)
            {
                Navigation.PushAsync(new Views.EditarProduto
                {
                    BindingContext = p,
                });
                lst_produtos.SelectedItem = null;
            }

        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        await AtualizarListaProdutos();
    }

    private void ExibirRelatorioGastosPorCategoria()
    {
        try
        {
            var gastosPorCategoria = lista.GroupBy(p => p.Categoria)
                .Select(g => new { Categoria = g.Key, Total = g.Sum(p => p.Total) });

            string relatorio = "Relatório de Gastos por Categoria:\n";
            foreach (var gasto in gastosPorCategoria)
            {
                relatorio += $"{gasto.Categoria}: {gasto.Total:C}\n";
            }

            lblRelatorioGastos.Text = relatorio;
        }
        catch (Exception ex)
        {
            lblRelatorioGastos.Text = $"Erro ao gerar relatório: {ex.Message}";
        }
    }
}
