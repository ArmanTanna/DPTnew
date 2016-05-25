#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    feature.cs
//
// Author:      Marco D'Alessandro
// Date:        July 2014
//
#endregion
using System;
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
  public class feature
  {
    private int? _featureIdentifier;

    #region public properties
    public string featureName { get; set; }
    public int featureIdentifier
    {
      get
      {
        if (this._featureIdentifier.HasValue)
        {
          return this._featureIdentifier.Value;
        }
        else
        {
          return default(int);
        }
      }
      set
      {
        this._featureIdentifier = value;
      }
    }
    [XmlIgnore()]
    public bool featureIdentifierSpecified
    {
      get
      {
        return this._featureIdentifier.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._featureIdentifier = null;
        }
      }
    }
    public string featureDescription { get; set; }
    public string refId1 { get; set; }
    public string refId2 { get; set; }
    #endregion
  }
}
