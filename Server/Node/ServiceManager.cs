﻿/*
    ------------------------------------------------------------------------------------
    LICENSE:
    ------------------------------------------------------------------------------------
    This file is part of EVE#: The EVE Online Server Emulator
    Copyright 2012 - Glint Development Group
    ------------------------------------------------------------------------------------
    This program is free software; you can redistribute it and/or modify it under
    the terms of the GNU Lesser General Public License as published by the Free Software
    Foundation; either version 2 of the License, or (at your option) any later
    version.

    This program is distributed in the hope that it will be useful, but WITHOUT
    ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
    FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License along with
    this program; if not, write to the Free Software Foundation, Inc., 59 Temple
    Place - Suite 330, Boston, MA 02111-1307, USA, or go to
    http://www.gnu.org/copyleft/lesser.txt.
    ------------------------------------------------------------------------------------
    Creator: Almamu
*/

using Common.Database;
using Common.Services;
using Node.Services.CacheSvc;
using Node.Services.Characters;
using Node.Services.Network;

namespace Node
{
    public class ServiceManager : Common.Services.ServiceManager
    {
        public NodeContainer Container { get; }
        public CacheStorage CacheStorage { get; }
        public objectCaching objectCaching { get; }
        public machoNet machoNet { get; }
        public alert alert { get; }
        public authentication authentication { get; }
        public character character { get; }
        private readonly DatabaseConnection mDatabaseConnection = null;

        public ServiceManager(NodeContainer container, DatabaseConnection db, CacheStorage storage, Configuration.General configuration)
        {
            this.Container = container;
            this.mDatabaseConnection = db;
            this.CacheStorage = storage;

            // initialize services
            this.machoNet = new machoNet(this);
            this.objectCaching = new objectCaching(container.Logger, this);
            this.alert = new alert(container.Logger, this);
            this.authentication = new authentication(configuration.Authentication, this);
            this.character = new character(storage, db, this);
        }
    }
}