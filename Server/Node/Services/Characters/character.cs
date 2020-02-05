/*
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marshal;
using Common;
using Common.Services;
using System.IO;
using Common.Database;
using Node.Database;

namespace Node.Services.Characters
{
    public class character : Service
    {
        private CharacterDB mDB = null;
        
        public character(DatabaseConnection db)
            : base("character")
        {
            this.mDB = new CharacterDB(db);
        }

        public PyObject GetCharactersToSelect(PyTuple args, Client client)
        {
            return this.mDB.GetCharacterList(client.AccountID);
        }

        public PyObject LogStartOfCharacterCreation(PyTuple args, Client client)
        {
            return null;
        }
    }
}