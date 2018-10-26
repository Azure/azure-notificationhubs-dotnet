$secretsPath = ".\UwpSample\~Secrets.cs"
((Get-Content -path $secretsPath -Raw) -replace '<HUB_NAME>',$env:HUB_NAME) | Set-Content -Path $secretsPath
((Get-Content -path $secretsPath -Raw) -replace '<HUB_CONNECTION_STRING>',$env:HUB_CONNECTION_STRING) | Set-Content -Path $secretsPath

$packageStoreAssociationXmlTemplatePath = ".\UwpSample\Package.StoreAssociation.xml.template"
$packageStoreAssociationXmlPath = ".\UwpSample\Package.StoreAssociation.xml"
((Get-Content -path $packageStoreAssociationXmlTemplatePath -Raw) -replace '__STORE_PUBLISHER__',$env:STORE_PUBLISHER) | Set-Content -Path $packageStoreAssociationXmlTemplatePath
((Get-Content -path $packageStoreAssociationXmlTemplatePath -Raw) -replace '__STORE_PUBLISHER_DISPLAY_NAME__',$env:STORE_PUBLISHER_DISPLAY_NAME) | Set-Content -Path $packageStoreAssociationXmlTemplatePath
((Get-Content -path $packageStoreAssociationXmlTemplatePath -Raw) -replace '__STORE_PACKAGE_IDENTITY_NAME__',$env:STORE_PACKAGE_IDENTITY_NAME) | Set-Content -Path $packageStoreAssociationXmlTemplatePath
((Get-Content -path $packageStoreAssociationXmlTemplatePath -Raw) -replace '__STORE_PACKAGE_RESERVED_NAME__',$env:STORE_PACKAGE_RESERVED_NAME) | Set-Content -Path $packageStoreAssociationXmlTemplatePath
((Get-Content -path $packageStoreAssociationXmlTemplatePath -Raw) -replace '__STORE_APP_ID__',$env:STORE_APP_ID) | Set-Content -Path $packageStoreAssociationXmlTemplatePath

Get-Content $packageStoreAssociationXmlTemplatePath | Set-Content -Path $packageStoreAssociationXmlPath