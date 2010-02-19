function clearASOCache( path ) {
  if (!FLfile.exists( path )) {
    fl.trace( path + "does not exist" );
    return;
  }
  FLfile.remove( path );
}

clearASOCache ( fl.configURI + "Classes/aso" );
fl.outputPanel.clear();
fl.getDocumentDOM().save();
fl.getDocumentDOM().publish();
fl.quit(false);