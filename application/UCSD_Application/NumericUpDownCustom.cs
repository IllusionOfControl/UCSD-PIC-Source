//*************************************************************************************************
//                                                                                                *
//      USB Computer Status Display                                                               *
//      Copyright Andrew Gehringer 2013                                                           *
//      Contact: projects@agehringer.com  --  www.AGehringer.com                                  *
//                                                                                                *
//------------------------------------------------------------------------------------------------*
//    This program is free software: you can redistribute it and/or modify it under the terms of  *
//    the GNU General Public License as published by the Free Software Foundation, version 3 of   *
//    the license.                                                                                *
//                                                                                                *
//    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY    *
//    without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.   *
//    See the GNU General Public License for more details.                                        *
//                                                                                                *
//    You should have received a copy of the GNU General Public License along with this program.  *
//    If not, see <http://www.gnu.org/licenses/>.                                                 *
//                                                                                                *
//*************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UCSD_Application
{
    public partial class NumericUpDownCustom : NumericUpDown
    {
        public NumericUpDownCustom()
        {
        }

        protected override void UpdateEditText()
        {
            if (this.Value == 0)
                this.Text = "Disabled";
            else
                base.UpdateEditText();

        }
    }
}
