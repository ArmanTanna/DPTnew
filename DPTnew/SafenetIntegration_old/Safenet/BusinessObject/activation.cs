#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    activation.cs
//
// Author:      Marco D'Alessandro
// Date:        July 2014
//
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SafenetIntegration.Safenet.BusinessObject
{
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public class activation
  {
    public activation()
    {
      this.activationInput = new activationInput();
      this.activationOutput = new activationOutput();
    }

    public activationOutput activationOutput { get; set; }

    public activationInput activationInput { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class activationOutput
  {
    public object aid { get; set; }

    public object protectionKeyId { get; set; }

    public object activationString { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class activationInput
  {
    private bool? _sendMail;

    public activationInput()
    {
      this.activationAttribute = new List<activationAttribute>();
    }

    [XmlElement("activationAttribute")]
    public List<activationAttribute> activationAttribute { get; set; }

    public string comments { get; set; }

    public bool sendMail
    {
      get
      {
        if (this._sendMail.HasValue)
        {
          return this._sendMail.Value;
        }
        else
        {
          return default(bool);
        }
      }
      set
      {
        this._sendMail = value;
      }
    }

    [XmlIgnore()]
    public bool sendMailSpecified
    {
      get
      {
        return this._sendMail.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._sendMail = null;
        }
      }
    }
  }
}
