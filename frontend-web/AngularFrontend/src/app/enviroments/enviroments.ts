// make url dynamic according to entered url
// for debugging on mobile devices
const host = window.location.hostname;

export const environment = {
  //production: false,
  API_BACKEND_JAVA: `http://${host}:8080`,
  API_BACKEND_DOTNET: `http://${host}:5200`
};
