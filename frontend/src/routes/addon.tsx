import { createFileRoute, Link } from '@tanstack/react-router'
import { apiRoot } from '../api/apiconfig';

export const Route = createFileRoute('/addon')({
  component: RouteComponent,
})

function RouteComponent() {

  return (
    <div>
        <Link to='/'>Back to home page</Link>
        <h1>Instructions</h1>
        <h2>In-game</h2>
        <ol>
          <li>Install the addon to extract your profession data: <a download href={`${apiRoot}/Files/addon`}>RecipeShare-0.1.7</a>.</li>
          <ul>
            <li>If you need help installing addons manually, see <a href='https://www.wowinterface.com/forums/faq.php?faq=install' target='_blank' rel='noopener noreferrer'>this guide</a>.</li>
            <li>In our case, we will use the <i style={{ fontFamily: 'monospace' }}>_classic_era_</i> folder.</li>
          </ul>
          <li>
            Once the addon is installed, check that you can load the addon in-game (Check your addons via the character select screen or in-game using the escape menu)
          </li>
          <li>If the addon is in the list and is enabled, continue along. If the addon warns that it is <strong style={{ fontFamily: 'monospace', color: 'red' }}>Out of date</strong>, make sure to check for an update here.</li>
          <li>Once in-game, open your profession window. This will cause the addon to read all your profession data for the opened window. Do this for each profession.</li>
          <li>Reload the game using the following command in your chat window: <span style={{ fontFamily: 'monospace' }}>/reload</span></li>
        </ol>
        <h2>On the website</h2>
        <span>If you are on this step, this means you have completed the previous instructions to extract your profession data from the game.</span>
        <ol>
          <li>Find your character via the realm drop-down menu on the <Link to='/'>home page</Link></li>
          <li>Open your character <i>details</i></li>
          <li>Select the <i>Update</i> tab</li>
          <li>Select <i>browse</i> and find the <b>RecipeShare.lua</b> file. This will be found in your WoW character's <i>SavedVariables</i> folder.</li>
          <ul>
            <li>ex: C:\Program Files\World of Warcraft\_classic_era_\WTF\Account\&lt;account_id&gt;\&lt;realm_name&gt;\&lt;char_name&gt;\SavedVariables\RecipeShare.lua</li>
            <li>Replace &lt;account_id&gt; with your account ID. If you have only one account, only one folder will be present.</li>
            <li>Replace &lt;realm_name&gt; with the realm your character is on. (i.e. Dreamscythe)</li>
            <li>Replace &lt;char_name&gt; with your character's name (i.e. Kyranai)</li>
          </ul>
          <li>
            <i>Submit</i> this file to update your character information.
          </li>
        </ol>
    </div>
  )
}
