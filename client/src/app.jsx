import domdom from '@eirikb/domdom';
import { UserAgentApplication } from 'msal';

const dd = domdom();

const scopes = ['api://80ce0b17-ac0b-43f5-add5-cd8c3412b6c9/Api.ReadWrite'];
const clientId = '80ce0b17-ac0b-43f5-add5-cd8c3412b6c9';
const apiUrl = 'https://intron-api.avento.no/api';
// For tenant you can either use tenant name/url or TenantId
const tenant = '*****.onmicrosoft.com';
const authority = `https://login.microsoftonline.com/${tenant}`;

const msal = new UserAgentApplication({ auth: { clientId, authority } });

async function callApi(path) {
  const { accessToken } = await msal.acquireTokenSilent({ scopes });
  return fetch(`${apiUrl}${path}`, {
    headers: {
      authorization: `Bearer ${accessToken}`
    }
  }).then(r => r.json());
}

async function callCustomApi() {
  dd.set('info', 'Fetching tokens...');
  try {
    dd.set('info', 'Calling custom API...');
    const values = await callApi('/api/Values');
    dd.set('info', `Values: ${values}`);
  } catch (e) {
    dd.set('info', e);
    throw e;
  }
}

msal.handleRedirectCallback(callCustomApi);

if (msal.getAccount()) {
  callCustomApi();
} else {
  msal.loginRedirect({ scopes });
}

const app = ({ on }) => <div>Welcome. <div>{on('info')}</div></div>;

dd.append(document.body, app);
