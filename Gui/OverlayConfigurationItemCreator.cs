using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ConfigGUI.ConfigurationRegion.ConfigurationItemCreators;
using Sync.Tools.ConfigurationAttribute;

namespace IngameOverlay.Gui
{
    class OverlayConfigurationItemCreator : BaseConfigurationItemCreator
    {
        private OverlayEditor editor;

        public override Panel CreateControl(BaseConfigurationAttribute attr, PropertyInfo prop, object configuration_instance)
        {
            var panel = base.CreateControl(attr, prop, configuration_instance);
            var btn = new Button()
            {
                Content = "Open Editor",
                Height = 25
            };
            btn.Click += (s, e) =>
            {
                editor = editor ?? new OverlayEditor();
                if (editor.Visibility == Visibility.Visible)
                    editor.Activate();
                else
                    editor.Show();
            };

            panel.Children.Add(btn);
            return panel;
        }
    }
}
