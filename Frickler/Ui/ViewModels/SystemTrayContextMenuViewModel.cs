﻿// Dapplo - building blocks for desktop applications
// Copyright (C) 2017-2018  Dapplo
// 
// For more information see: http://dapplo.net/
// Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
// This file is part of Frickler
// 
// Frickler is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Frickler is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have a copy of the GNU Lesser General Public License
// along with Frickler. If not, see <http://www.gnu.org/licenses/lgpl.txt>.
// 

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Autofac.Features.AttributeFilters;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.CaliburnMicro.NotifyIconWpf;
using Dapplo.CaliburnMicro.NotifyIconWpf.ViewModels;
using Dapplo.Frickler.Configuration;
using MahApps.Metro.IconPacks;

#endregion

namespace Dapplo.Frickler.Ui.ViewModels
{
    /// <summary>
    ///     This take care of the tray icon and context-menu
    /// </summary>
    public class SystemTrayContextMenuViewModel : TrayIconViewModel
    {
        private readonly IEnumerable<Lazy<IMenuItem>> _contextMenuItems;
        private readonly IContextMenuTranslations _contextMenuTranslations;

        /// <summary>
        ///     Construct the SystemTrayContextMenuViewModel with it's dependencies
        /// </summary>
        /// <param name="contextMenuTranslations">IContextMenuTranslations</param>
        /// <param name="trayIconManager">ITrayIconManager</param>
        /// <param name="contextMenuItems">IEnumerable of IMenuItem</param>
        public SystemTrayContextMenuViewModel(
            IContextMenuTranslations contextMenuTranslations,
            ITrayIconManager trayIconManager,
            [MetadataFilter("Menu","systemtray")] IEnumerable<Lazy<IMenuItem>> contextMenuItems) : base(trayIconManager)
        {
            _contextMenuTranslations = contextMenuTranslations;
            _contextMenuItems = contextMenuItems;
        }

        /// <inheritdoc />
        protected override void OnActivate()
        {
            base.OnActivate();

            // Set the title of the icon (the ToolTipText) to our IContextMenuTranslations.Title
            _contextMenuTranslations.CreateDisplayNameBinding(this, nameof(IContextMenuTranslations.Title));

            var items = new List<IMenuItem>();

            // Lazy values
            items.AddRange(_contextMenuItems.Select(lazy => lazy.Value));

            var titleItem = new MenuItem
            {
                Id = "A_Title",
                Style = MenuItemStyles.Title,
                Icon = new PackIconMaterial
                {
                    Kind = PackIconMaterialKind.Network,
                    Background = Brushes.White,
                    Foreground = Brushes.Black
                }
            };
            _contextMenuTranslations.CreateDisplayNameBinding(titleItem, nameof(IContextMenuTranslations.Title));
            titleItem.ApplyIconForegroundColor(Brushes.DarkRed);
            items.Add(titleItem);
            items.Add(new MenuItem
            {
                Style = MenuItemStyles.Separator,
                Id = "Y_Separator"
            });

            // Create an exit item
            var exitItem = new ClickableMenuItem
            {
                Id = "Z_Exit",
                Icon = new PackIconMaterial
                {
                    Kind = PackIconMaterialKind.Close,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch
                },
                ClickAction = clickedItem => { Application.Current.Shutdown(); }
            };
            exitItem.ApplyIconForegroundColor(Brushes.DarkRed);
            _contextMenuTranslations.CreateDisplayNameBinding(exitItem, nameof(IContextMenuTranslations.Exit));
            items.Add(exitItem);

            ConfigureMenuItems(items);

            // Make sure the margin is set, do this AFTER the icon are set
            items.ApplyIconMargin(new Thickness(2, 2, 2, 2));

            SetIcon(new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Network,
                Background = Brushes.White,
                Foreground = Brushes.Black
            });
            Show();
        }
    }
}