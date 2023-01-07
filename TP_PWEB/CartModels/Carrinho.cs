using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml;
using A.Models;

namespace A.CartModels
{
    public class Carrinho
    {
        public List<CarrinhoItem> items { get; set; } = new List<CarrinhoItem>();
        public void AddItem(CarrinhoItem reserva, int qtd)
        {
            CarrinhoItem item = items.Where(b => b.ReservaId == reserva.ReservaId).FirstOrDefault();
            if (item == null)
            {
                items.Add(new CarrinhoItem
                {
                    ReservaId = reserva.ReservaId,
                    Quantidade = qtd,
                    PrecoUnit = reserva.PrecoUnit
                    
                });
            }
            else
            {
                item.Quantidade += qtd;
            }
        }

        public void RemoveItem(CarrinhoItem reserva) => items.RemoveAll(l => l.ReservaId == reserva.ReservaId);
        public decimal Total() => items.Sum(e => e.PrecoUnit * e.Quantidade);
        public void Clear() => items.Clear();

    }
}
