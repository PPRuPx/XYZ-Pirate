using Model;
using Model.Definitions;
using Model.Definitions.Localization;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.ShopWindow
{
    public class ShopItemWidget : MonoBehaviour, IItemRenderer<ShopItemDef>
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
        [SerializeField] private GameObject _selector;
        
        private GameSession _session;
        private ShopItemDef _data;
        
        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            UpdateView();
        }
        
        public void SetData(ShopItemDef data, int index)
        {
            _data = data;
            if (_session != null)
                UpdateView();
        }

        private void UpdateView()
        {
            var shopModel = _session.ShopModel;
            
            _icon.sprite = _data.Icon;
            _name.text = LocalizationManager.I.Localize(_data.Id);
            _selector.SetActive(_session.ShopModel.InterfaceSelection.Value == _data.Id);
        }
        
        public void OnSelect()
        {
            _session.ShopModel.InterfaceSelection.Value = _data.Id;
        }
    }
}